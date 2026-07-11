import sys
import types
import unittest
from decimal import Decimal
from pathlib import Path


sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

fastapi_stub = types.ModuleType("fastapi")


class HTTPException(Exception):
    def __init__(self, status_code: int, detail: str) -> None:
        super().__init__(detail)
        self.status_code = status_code
        self.detail = detail


fastapi_stub.HTTPException = HTTPException
sys.modules.setdefault("fastapi", fastapi_stub)

config_stub = types.ModuleType("app.config")
config_stub.settings = types.SimpleNamespace(openai_api_key=None, gemini_api_key=None)
sys.modules.setdefault("app.config", config_stub)
xlrd_stub = types.ModuleType("xlrd")
xlrd_stub.XL_CELL_DATE = 3
xlrd_stub.open_workbook = lambda *args, **kwargs: None
xlrd_stub.xldate_as_tuple = lambda *args, **kwargs: (1970, 1, 1, 0, 0, 0)
sys.modules.setdefault("xlrd", xlrd_stub)

from app.import_ai import manual_transform_transactions


SERVICES = [
    {"id": 1, "name": "Cash Service", "transaction_type": "IN & OUT", "switch_type": None},
    {"id": 2, "name": "Bill Service", "transaction_type": "IN", "switch_type": None},
]

RULES = [
    {
        "id": "r-out",
        "enabled": True,
        "matchType": "contains",
        "pattern": "withdraw",
        "serviceId": "1",
        "direction": "OUT",
        "caseSensitive": False,
    },
    {
        "id": "r-in",
        "enabled": True,
        "matchType": "contains",
        "pattern": "deposit",
        "serviceId": "1",
        "direction": "IN",
        "caseSensitive": False,
    },
    {
        "id": "r-bill",
        "enabled": True,
        "matchType": "contains",
        "pattern": "bill",
        "serviceId": "2",
        "direction": "OUT",
        "caseSensitive": False,
    },
]


class ManualImportTests(unittest.TestCase):
    def test_manual_rule_controls_service_and_direction(self) -> None:
        rows = [
            ["Date", "Description", "Debit", "Credit", "Solde"],
            ["10/07/2026", "client withdraw", "", "1 200,50", "9 000,00"],
            ["10/07/2026", "client deposit", "-300,25", "", "8 699,75"],
            ["10/07/2026", "bill payment", "", "99,99", "8 599,76"],
        ]

        result = manual_transform_transactions(rows, RULES, SERVICES, ["IN", "OUT"])

        self.assertEqual([row["service"] for row in result], ["Cash Service", "Cash Service", "Bill Service"])
        self.assertEqual([row["direction"] for row in result], ["OUT", "IN", "OUT"])
        self.assertEqual([row["amount"] for row in result], ["1200.50", "300.25", "99.99"])
        self.assertEqual(result[-1]["solde"], "8599.76")

    def test_unmatched_and_ambiguous_rows_are_flagged_for_review(self) -> None:
        rows = [
            ["Description", "Debit", "Credit", "Solde"],
            ["no matching rule", "", "44.00", "100.00"],
            ["bad both columns", "10.00", "20.00", "90.00"],
            ["bad empty columns", "", "", "90.00"],
        ]

        result = manual_transform_transactions(rows, RULES, SERVICES, ["IN", "OUT"])

        self.assertEqual([row["kind"] for row in result], ["unknown", "unknown", "unknown"])
        self.assertIn("No manual rule matched", str(result[0]["error_message"]))
        self.assertIn("Both debit and credit", str(result[1]["error_message"]))
        self.assertIn("Both debit and credit are empty", str(result[2]["error_message"]))

    def test_review_totals_match_manual_rule_directions(self) -> None:
        rows = [
            ["Description", "Debit", "Credit", "Solde"],
            ["client withdraw", "", "100.00", "900.00"],
            ["client deposit", "25.00", "", "925.00"],
        ]

        result = manual_transform_transactions(rows, RULES, SERVICES, ["IN", "OUT"])
        totals = {"IN": Decimal("0"), "OUT": Decimal("0")}
        for row in result:
            if row["kind"] == "service":
                totals[str(row["direction"])] += Decimal(str(row["amount"]))

        self.assertEqual(totals["IN"], Decimal("25.00"))
        self.assertEqual(totals["OUT"], Decimal("100.00"))
        self.assertEqual(result[-1]["solde"], "925.00")

    def test_frais_in_opposite_column_uses_opposite_previous_type(self) -> None:
        rules = [
            {
                "id": "r-cnss",
                "enabled": True,
                "matchType": "contains",
                "pattern": "mad cnss",
                "serviceId": "2",
                "direction": "IN",
                "caseSensitive": False,
            }
        ]
        rows = [
            ["Date", "Description", "Debit", "Credit", "Solde"],
            ["04/07/2026 13:59", "Paiement mad cnss", "", "196,00", "361.810,02"],
            ["04/07/2026 13:59", "Frais mad cnss", "15", "", "361.795,02"],
        ]

        result = manual_transform_transactions(rows, rules, SERVICES, ["IN", "OUT"])

        self.assertEqual(len(result), 2)
        self.assertEqual(result[0]["description"], "Paiement mad cnss")
        self.assertEqual(result[0]["direction"], "IN")
        self.assertEqual(result[0]["amount"], "196.00")
        self.assertEqual(result[0]["fee"], "")
        self.assertEqual(result[1]["description"], "Frais mad cnss")
        self.assertEqual(result[1]["direction"], "OUT")
        self.assertEqual(result[1]["amount"], "15.00")
        self.assertEqual(result[1]["fee"], "")

if __name__ == "__main__":
    unittest.main()
