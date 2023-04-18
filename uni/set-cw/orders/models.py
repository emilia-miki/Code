from typing import NamedTuple
from enum import IntEnum


class OrderStatus(IntEnum):
    Created = 0
    Taken = 1
    InProgress = 2
    Finished = 3
    Published = 4
    Canceled = 5


class Order(NamedTuple):
    id: str
    customer_id: str
    target_audience: str
    audience_size: int
    budget: float
    template_id: str
    style_id: str
    additional_info: str
    status: OrderStatus
