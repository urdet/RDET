from __future__ import annotations

import csv
import io
import json
import re
import urllib.error
import urllib.request
import zipfile
from datetime import datetime
from decimal import Decimal, InvalidOperation
from typing import Any
from xml.etree import ElementTree

from fastapi import HTTPException
import xlrd

from app.config import settings


NS = {"a": "http://schemas.openxmlformats.org/spreadsheetml/2006/main"}
DEFAULT_OPENAI_MODEL = "gpt-4.1-mini"
DEFAULT_GEMINI_MODEL = "gemini-2.5-flash"


def clean_cell(value: str | None) -> str:
    return (value or "").replace("\n", " ").strip()


def parse_csv(content: bytes) -> list[list[str]]:
    text = content.decode("utf-8-sig", errors="replace")
    sample = text[:2048]
    dialect = csv.Sniffer().sniff(sample, delimiters=",;\t|") if sample.strip() else csv.excel
    return [[clean_cell(cell) for cell in row] for row in csv.reader(io.StringIO(text), dialect)]


def column_index(cell_ref: str) -> int:
    letters = re.sub(r"[^A-Z]", "", cell_ref.upper())
    index = 0
    for letter in letters:
        index = index * 26 + (ord(letter) - ord("A") + 1)
    return max(index - 1, 0)


def parse_xlsx(content: bytes) -> list[list[str]]:
    rows: list[list[str]] = []
    with zipfile.ZipFile(io.BytesIO(content)) as archive:
        shared_strings: list[str] = []
        if "xl/sharedStrings.xml" in archive.namelist():
            root = ElementTree.fromstring(archive.read("xl/sharedStrings.xml"))
            for item in root.findall("a:si", NS):
                parts = [node.text or "" for node in item.findall(".//a:t", NS)]
                shared_strings.append(clean_cell("".join(parts)))

        sheet_name = next((name for name in archive.namelist() if name.startswith("xl/worksheets/sheet") and name.endswith(".xml")), None)
        if not sheet_name:
            return rows
        root = ElementTree.fromstring(archive.read(sheet_name))
        for row in root.findall(".//a:sheetData/a:row", NS):
            values: list[str] = []
            for cell in row.findall("a:c", NS):
                ref = cell.attrib.get("r", "")
                while len(values) <= column_index(ref):
                    values.append("")
                cell_type = cell.attrib.get("t")
                value_node = cell.find("a:v", NS)
                inline_node = cell.find("a:is/a:t", NS)
                value = clean_cell(inline_node.text if inline_node is not None else value_node.text if value_node is not None else "")
                if cell_type == "s" and value:
                    try:
                        value = shared_strings[int(value)]
                    except (ValueError, IndexError):
                        pass
                values[column_index(ref)] = value
            if any(values):
                rows.append(values)
    return rows


def parse_xls(content: bytes) -> list[list[str]]:
    book = xlrd.open_workbook(file_contents=content)
    sheet = book.sheet_by_index(0)
    rows: list[list[str]] = []
    for row_index in range(sheet.nrows):
        values: list[str] = []
        for col_index in range(sheet.ncols):
            cell = sheet.cell(row_index, col_index)
            if cell.ctype == xlrd.XL_CELL_DATE:
                values.append(datetime(*xlrd.xldate_as_tuple(cell.value, book.datemode)).isoformat())
            else:
                values.append(clean_cell(str(cell.value)))
        if any(values):
            rows.append(values)
    return rows


def spreadsheet_rows(filename: str, content: bytes) -> list[list[str]]:
    lower = filename.lower()
    if lower.endswith(".csv"):
        return parse_csv(content)
    if lower.endswith(".xlsx"):
        return parse_xlsx(content)
    if lower.endswith(".xls"):
        return parse_xls(content)
    raise HTTPException(status_code=400, detail="Import supports .xlsx, .xls or .csv files")


def table_excerpt(rows: list[list[str]], max_rows: int = 80, max_cols: int = 14) -> str:
    excerpt = rows[:max_rows]
    lines = []
    for index, row in enumerate(excerpt, start=1):
        cells = [clean_cell(cell) for cell in row[:max_cols]]
        lines.append(f"{index}: " + " | ".join(cells))
    return "\n".join(lines)


def normalize_amount(value: Any) -> str:
    text = str(value or "").strip().replace(" ", "").replace(",", ".")
    text = re.sub(r"[^0-9.\-]", "", text)
    try:
        amount = Decimal(text)
    except InvalidOperation:
        return ""
    if amount <= 0:
        return ""
    return format(amount.quantize(Decimal("0.01")), "f")


def normalize_balance(value: Any) -> str:
    text = str(value or "").strip().replace(" ", "").replace(",", ".")
    text = re.sub(r"[^0-9.\-]", "", text)
    try:
        amount = Decimal(text)
    except InvalidOperation:
        return ""
    return format(amount.quantize(Decimal("0.01")), "f")


def normalize_occurred_at(value: Any) -> str | None:
    text = clean_cell(str(value or ""))
    if not text:
        return None
    for fmt in ("%Y-%m-%d %H:%M:%S", "%Y-%m-%d %H:%M", "%Y-%m-%d", "%d/%m/%Y %H:%M:%S", "%d/%m/%Y %H:%M", "%d/%m/%Y"):
        try:
            return datetime.strptime(text.replace("T", " "), fmt).isoformat()
        except ValueError:
            pass
    return text if "T" in text else None


def extract_output_text(response: dict[str, Any]) -> str:
    if isinstance(response.get("output_text"), str):
        return response["output_text"]
    chunks: list[str] = []
    for item in response.get("output", []):
        for content in item.get("content", []):
            text = content.get("text")
            if isinstance(text, str):
                chunks.append(text)
    return "".join(chunks)


def transaction_import_schema(allowed_directions: list[str]) -> dict[str, Any]:
    return {
        "type": "object",
        "additionalProperties": False,
        "properties": {
            "rows": {
                "type": "array",
                "items": {
                    "type": "object",
                    "additionalProperties": False,
                    "properties": {
                        "service": {"type": "string"},
                        "kind": {"type": "string", "enum": ["service", "charge", "unknown"]},
                        "direction": {"type": "string", "enum": allowed_directions},
                        "amount": {"type": "string"},
                        "fee": {"type": "string"},
                        "solde": {"type": "string"},
                        "occurred_at": {"type": ["string", "null"]},
                        "description": {"type": ["string", "null"]},
                        "source_row_number": {"type": "integer"},
                    },
                    "required": ["service", "kind", "direction", "amount", "fee", "solde", "occurred_at", "description", "source_row_number"],
                },
            }
        },
        "required": ["rows"],
    }


def import_system_prompt(prompt: str | None) -> str:
    return (
        "You transform spreadsheet rows into import rows. Detect the service from each row. "
        "Use kind=service only when it matches an available service. Use kind=charge for agency expenses/charges paid out. "
        "Use kind=unknown for operations whose service is not in the available services and is not a charge. "
        "Return only valid JSON matching the schema. Amount and fee must be positive decimal strings. "
        "If the row has a balance after operation, return it as solde; solde may be zero or negative. If absent, use empty string. "
        "The JSON root must be an object with a rows array, not a bare array. "
        "If a row has a date or time, return occurred_at as ISO 8601. If not, use null. "
        "Set source_row_number to the 1-based spreadsheet row number shown before the row. "
        "Ignore headers, empty rows, totals, balances, and rows without a transaction amount.\n"
        f"User import rules:\n{prompt or 'No additional rules.'}"
    )


def import_user_prompt(rows: list[list[str]], service_names: list[str], allowed_directions: list[str], service_types: dict[str, str] | None = None) -> str:
    service_lines = [
        f"- {name} | accepted type: {(service_types or {}).get(name, 'IN & OUT')}"
        for name in service_names
    ]
    return (
        "Available services. Return service exactly as one of these names and respect its accepted type:\n"
        f"{chr(10).join(service_lines)}\n"
        f"Allowed directions: {', '.join(allowed_directions)}\n"
        "Spreadsheet rows:\n"
        f"{table_excerpt(rows)}"
    )


def openai_transform_transactions(
    rows: list[list[str]],
    service_names: list[str],
    allowed_directions: list[str],
    prompt: str | None = None,
    api_key: str | None = None,
    model: str | None = None,
    service_types: dict[str, str] | None = None,
) -> list[dict[str, str | None]]:
    key = api_key or settings.openai_api_key
    if not key:
        raise HTTPException(status_code=400, detail="OpenAI API key is not configured")

    payload = {
        "model": model or settings.openai_model or DEFAULT_OPENAI_MODEL,
        "input": [
            {"role": "system", "content": import_system_prompt(prompt)},
            {"role": "user", "content": import_user_prompt(rows, service_names, allowed_directions, service_types)},
        ],
        "text": {"format": {"type": "json_schema", "name": "transaction_import", "schema": transaction_import_schema(allowed_directions), "strict": True}},
    }
    request = urllib.request.Request(
        "https://api.openai.com/v1/responses",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Authorization": f"Bearer {key}", "Content-Type": "application/json"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(request, timeout=settings.openai_timeout_seconds) as response:
            body = json.loads(response.read().decode("utf-8"))
    except urllib.error.HTTPError as exc:
        detail = exc.read().decode("utf-8", errors="replace")
        raise HTTPException(status_code=502, detail=f"OpenAI import failed: {detail[:500]}") from exc
    except Exception as exc:
        raise HTTPException(status_code=502, detail=f"OpenAI import failed: {exc}") from exc

    output_text = extract_output_text(body)
    try:
        parsed = json.loads(output_text)
    except json.JSONDecodeError as exc:
        raise HTTPException(status_code=502, detail="OpenAI returned unreadable import data") from exc

    return normalize_import_rows(parsed, service_names, allowed_directions)


def gemini_transform_transactions(
    rows: list[list[str]],
    service_names: list[str],
    allowed_directions: list[str],
    prompt: str | None = None,
    api_key: str | None = None,
    model: str | None = None,
    service_types: dict[str, str] | None = None,
) -> list[dict[str, str | None]]:
    key = api_key or settings.gemini_api_key
    if not key:
        raise HTTPException(status_code=400, detail="Google Gemini API key is not configured")
    model_name = model or settings.gemini_model or DEFAULT_GEMINI_MODEL
    payload = {
        "contents": [{"role": "user", "parts": [{"text": f"{import_system_prompt(prompt)}\n\n{import_user_prompt(rows, service_names, allowed_directions, service_types)}"}]}],
        "generationConfig": {"responseMimeType": "application/json"},
    }
    request = urllib.request.Request(
        f"https://generativelanguage.googleapis.com/v1beta/models/{model_name}:generateContent?key={key}",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(request, timeout=settings.gemini_timeout_seconds) as response:
            body = json.loads(response.read().decode("utf-8"))
    except urllib.error.HTTPError as exc:
        detail = exc.read().decode("utf-8", errors="replace")
        raise HTTPException(status_code=502, detail=f"Gemini import failed: {detail[:500]}") from exc
    except TimeoutError as exc:
        raise HTTPException(status_code=504, detail="Gemini import timed out. Try a smaller file, fewer rows, or a faster Gemini model.") from exc
    except Exception as exc:
        raise HTTPException(status_code=502, detail=f"Gemini import failed: {exc}") from exc

    output_text = "".join(
        part.get("text", "")
        for candidate in body.get("candidates", [])
        for part in candidate.get("content", {}).get("parts", [])
        if isinstance(part.get("text"), str)
    )
    try:
        parsed = json.loads(output_text)
    except json.JSONDecodeError as exc:
        raise HTTPException(status_code=502, detail="Gemini returned unreadable import data") from exc
    return normalize_import_rows(parsed, service_names, allowed_directions)


def transform_transactions(
    provider: str,
    rows: list[list[str]],
    service_names: list[str],
    allowed_directions: list[str],
    prompt: str | None = None,
    api_key: str | None = None,
    model: str | None = None,
    service_types: dict[str, str] | None = None,
) -> list[dict[str, str | None]]:
    if provider == "google_gemini":
        return gemini_transform_transactions(rows, service_names, allowed_directions, prompt, api_key, model, service_types)
    return openai_transform_transactions(rows, service_names, allowed_directions, prompt, api_key, model, service_types)


def normalize_header(value: Any) -> str:
    text = clean_cell(str(value or "")).lower()
    replacements = {
        "é": "e",
        "è": "e",
        "ê": "e",
        "à": "a",
        "â": "a",
        "ù": "u",
        "û": "u",
        "î": "i",
        "ï": "i",
        "ô": "o",
        "ç": "c",
    }
    for source, target in replacements.items():
        text = text.replace(source, target)
    return re.sub(r"[^a-z0-9]+", " ", text).strip()


def manual_header_map(rows: list[list[str]]) -> tuple[int, dict[str, int]]:
    candidates: dict[str, tuple[set[str], set[str]]] = {
        "description": ({"description", "desc", "libelle", "motif", "operation", "details", "detail", "designation", "narration", "reference"}, set()),
        "amount": ({"amount", "montant", "somme", "total", "transaction amount"}, set()),
        "credit": ({"credit", "credite", "versement", "entree", "in", "cash in"}, set()),
        "debit": ({"debit", "debite", "retrait", "sortie", "out", "cash out"}, set()),
        "fee": ({"fee", "fees", "frais", "commission", "charge"}, set()),
        "solde": ({"solde", "balance", "bal", "new balance"}, set()),
        "date": ({"date", "datetime", "time", "heure", "created at", "operation date"}, set()),
    }
    best_row = 0
    best_map: dict[str, int] = {}
    best_score = 0
    for row_index, row in enumerate(rows[:15]):
        found: dict[str, int] = {}
        for col_index, cell in enumerate(row):
            header = normalize_header(cell)
            if not header:
                continue
            for key, (exact_values, _) in candidates.items():
                if key in found:
                    continue
                if header in exact_values or any(part in exact_values for part in header.split()):
                    found[key] = col_index
        score = len(found)
        if score > best_score:
            best_row = row_index
            best_map = found
            best_score = score
    return (best_row, best_map) if best_score >= 2 else (-1, {})


def row_text_description(row: list[str]) -> str:
    text_cells = [
        clean_cell(cell)
        for cell in row
        if clean_cell(cell) and not normalize_amount(cell) and not normalize_occurred_at(cell)
    ]
    return max(text_cells, key=len, default="")


def manual_rule_matches(description: str, rule: dict[str, Any]) -> bool:
    pattern = clean_cell(str(rule.get("pattern") or ""))
    if not pattern:
        return False
    case_sensitive = bool(rule.get("caseSensitive"))
    value = description if case_sensitive else description.lower()
    needle = pattern if case_sensitive else pattern.lower()
    match_type = rule.get("matchType") or "contains"
    if match_type == "equals":
        return value == needle
    if match_type == "starts_with":
        return value.startswith(needle)
    if match_type == "ends_with":
        return value.endswith(needle)
    if match_type == "regex":
        flags = 0 if case_sensitive else re.IGNORECASE
        try:
            return re.search(pattern, description, flags) is not None
        except re.error:
            return False
    return needle in value


def is_fee_description(description: str) -> bool:
    normalized = normalize_header(description)
    if not normalized:
        return False
    fee_words = {"frais", "fee", "fees", "commission", "commissions", "charge", "charges"}
    words = set(normalized.split())
    return bool(words & fee_words) or normalized.startswith(("frais ", "fee ", "commission ", "charge "))


def merge_fee_into_previous_row(transformed_rows: list[dict[str, str | int | None]], amount: str, description: str, service_name: str | None = None) -> bool:
    if not amount:
        return False
    for previous in reversed(transformed_rows[-4:]):
        if previous.get("kind") != "service":
            continue
        if service_name and previous.get("service") != service_name:
            continue
        existing_fee = normalize_amount(previous.get("fee")) or "0"
        merged_fee = Decimal(existing_fee) + Decimal(amount)
        previous["fee"] = format(merged_fee.quantize(Decimal("0.01")), "f")
        previous_description = clean_cell(str(previous.get("description") or ""))
        if description and description.lower() not in previous_description.lower():
            previous["description"] = clean_cell(f"{previous_description} | {description}") if previous_description else description
        return True
    return False


def manual_transform_transactions(
    rows: list[list[str]],
    rules: list[dict[str, Any]],
    services: list[dict[str, Any]],
    allowed_directions: list[str],
) -> list[dict[str, str | int | None]]:
    header_row, headers = manual_header_map(rows)
    data_rows = rows[header_row + 1 :] if header_row >= 0 else rows
    service_by_id = {str(service.get("id")): service for service in services}
    transformed_rows: list[dict[str, str | int | None]] = []

    for offset, row in enumerate(data_rows, start=header_row + 2 if header_row >= 0 else 1):
        description = clean_cell(row[headers["description"]]) if "description" in headers and headers["description"] < len(row) else row_text_description(row)
        if not description:
            continue

        credit = normalize_amount(row[headers["credit"]]) if "credit" in headers and headers["credit"] < len(row) else ""
        debit = normalize_amount(row[headers["debit"]]) if "debit" in headers and headers["debit"] < len(row) else ""
        amount = ""
        direction = "IN"
        if credit:
            amount = credit
            direction = "IN"
        elif debit:
            amount = debit
            direction = "OUT"
        elif "amount" in headers and headers["amount"] < len(row):
            raw_amount = clean_cell(row[headers["amount"]])
            amount = normalize_amount(raw_amount)
            direction = "OUT" if raw_amount.strip().startswith("-") else "IN"
        else:
            numeric_values = [normalize_amount(cell) for cell in row]
            amount = next((value for value in numeric_values if value), "")

        if direction not in allowed_directions or not amount:
            continue

        matched_rule = next((rule for rule in rules if rule.get("enabled", True) and manual_rule_matches(description, rule)), None)
        service = service_by_id.get(str(matched_rule.get("serviceId"))) if matched_rule else None
        service_name = clean_cell(str(service.get("name") or "")) if service else ""
        if is_fee_description(description) and merge_fee_into_previous_row(transformed_rows, amount, description, service_name or None):
            continue

        service_type = clean_cell(str(service.get("transaction_type") or service.get("switch_type") or "IN & OUT")) if service else "IN & OUT"
        rule_direction = matched_rule.get("direction") if matched_rule and matched_rule.get("direction") in allowed_directions else None
        if rule_direction:
            direction = str(rule_direction)
        elif service_type in {"IN", "OUT"}:
            direction = service_type

        fee = normalize_amount(row[headers["fee"]]) if "fee" in headers and headers["fee"] < len(row) else ""
        solde = normalize_balance(row[headers["solde"]]) if "solde" in headers and headers["solde"] < len(row) else ""
        occurred_at = normalize_occurred_at(row[headers["date"]]) if "date" in headers and headers["date"] < len(row) else None
        transformed_rows.append(
            {
                "kind": "service" if service else "unknown",
                "service": service_name,
                "direction": direction,
                "amount": amount,
                "fee": fee,
                "solde": solde,
                "occurred_at": occurred_at,
                "description": description,
                "source_row_number": offset,
            }
        )
    return transformed_rows


def normalize_import_rows(parsed: dict[str, Any] | list[Any], service_names: list[str], allowed_directions: list[str]) -> list[dict[str, str | None]]:
    normalized_rows: list[dict[str, str | None]] = []
    parsed_rows = parsed.get("rows", []) if isinstance(parsed, dict) else parsed
    if not isinstance(parsed_rows, list):
        raise HTTPException(status_code=502, detail="AI returned import data in an unexpected format")
    for row in parsed_rows:
        if not isinstance(row, dict):
            continue
        direction = row.get("direction")
        service = clean_cell(row.get("service"))
        kind = row.get("kind") if row.get("kind") in {"service", "charge", "unknown"} else "unknown"
        amount = normalize_amount(row.get("amount"))
        if direction not in allowed_directions or not amount:
            continue
        if kind == "service" and service not in service_names:
            kind = "unknown"
        try:
            source_row_number = int(row.get("source_row_number") or 0)
        except (TypeError, ValueError):
            source_row_number = 0
        normalized_rows.append(
            {
                "kind": kind,
                "service": service,
                "direction": direction,
                "amount": amount,
                "fee": normalize_amount(row.get("fee")) or "",
                "solde": normalize_balance(row.get("solde")),
                "occurred_at": normalize_occurred_at(row.get("occurred_at")),
                "description": clean_cell(row.get("description")) or None,
                "source_row_number": source_row_number,
            }
        )
    return normalized_rows
