from datetime import datetime, timedelta, timezone

from jose import jwt
from passlib.exc import PasswordSizeError, UnknownHashError
from passlib.context import CryptContext
from sqlalchemy.exc import SQLAlchemyError
from sqlalchemy import text
from sqlalchemy.orm import Session

from app.config import settings
from app.models import User

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
ALGORITHM = "HS256"


def verify_password(db: Session, password: str, password_hash: str) -> bool:
    if not password_hash:
        return False
    try:
        result = db.execute(text("SELECT crypt(:password, :hash) = :hash"), {"password": password, "hash": password_hash})
        if result.scalar():
            return True
    except SQLAlchemyError:
        db.rollback()

    try:
        return pwd_context.verify(password[:72], password_hash) if password_hash.startswith("$2") else pwd_context.verify(password, password_hash)
    except (PasswordSizeError, UnknownHashError, ValueError, TypeError):
        return False


def hash_password(db: Session, password: str) -> str:
    try:
        result = db.execute(text("SELECT crypt(:password, gen_salt('bf'))"), {"password": password})
        hashed = result.scalar_one_or_none()
        if hashed:
            return hashed
    except SQLAlchemyError:
        db.rollback()
    return pwd_context.hash(password[:72])


def create_access_token(user: User) -> str:
    expires = datetime.now(timezone.utc) + timedelta(minutes=settings.access_token_expire_minutes)
    payload = {"sub": str(user.id), "role": user.role.value, "exp": expires}
    return jwt.encode(payload, settings.secret_key, algorithm=ALGORITHM)
