from typing import NamedTuple
from enum import IntEnum


class AdFormat(IntEnum):
    MagazineText = 0
    MagazineTextWithMedia = 1
    MagazinePhoto = 2
    Billboard = 3
    WebVideo = 4
    WebImage = 5


class Template(NamedTuple):
    id: str
    html_id: str
    style_ids: list[str]
    format: AdFormat
    price: float


class Html(NamedTuple):
    id: str
    content: str


class Style(NamedTuple):
    id: str
    content: str


class Partner(NamedTuple):
    name: str
    contact: str
    audience_size: int
    coef: float
    trust_level: int


class FieldOfService(NamedTuple):
    name: str
