import redis
from .models import *
from enum import Enum, auto


class RedisDbStatus(Enum):
    Ok = auto()
    Order_already_exists = auto()
    Reference_to_non_existing_order = auto()


class OrdersDB:
    def pre_init(self):
        check_for_first_init = self.redis_db.hexists("orders_basic_stats", "order_count")
        if check_for_first_init == 1:
            self.last_order_num = int(self.redis_db.hget("orders_basic_stats", "order_count"))
        else:
            self.redis_db.hset("orders_basic_stats", "order_count", str(-1))

    def __init__(self, host="127.0.0.1", port=6379):
        self.redis_db = redis.StrictRedis(
            host=host,
            port=port,
            decode_responses=True,
            db=3
        )
        self.last_order_num = -1

        self.pre_init()

    def _settle_order(self, operation_order_id: str, operation_order: Order):
        res = operation_order.status
        self.redis_db.hset(
            name=operation_order_id,
            items=[
                "Customer ID", operation_order.customer_id,
                "Target audience", str(operation_order.target_audience),
                "Audience size", operation_order.audience_size,
                "Budget", operation_order.budget,
                "Template ID", str(operation_order.template_id),
                "Style ID", str(operation_order.style_id),
                "Additional info", operation_order.additional_info,
                "Status", operation_order.status
            ]
        )

        return res

    def add_order(self, order: Order):
        res = self.redis_db.hexists(str(self.last_order_num + 1), "Status")
        if res == 1:
            return None
        else:
            self.last_order_num += 1
            self._settle_order(str(self.last_order_num), order)
            self.redis_db.hset("orders_basic_stats", "order_count", str(self.last_order_num))
            return str(self.last_order_num)

    def change_order(self, order: Order):
        res = self.redis_db.hexists(order.id, "Status")
        if res == 1:
            self._settle_order(order.id, order)
            return True
        else:
            return False

    def change_order_status(self, operation_order_id: str, new_status: OrderStatus):
        res = self.redis_db.hexists(operation_order_id, "Status")
        if res == 1:
            self.redis_db.hset(operation_order_id, "Status", str(new_status))
            return RedisDbStatus.Ok
        else:
            return RedisDbStatus.Reference_to_non_existing_order

    def delete_order(self, operation_order_id: str):
        res = self.redis_db.hexists(operation_order_id, "Status")
        if res == 1:
            self.redis_db.delete(operation_order_id)
            return "order deleted"
        else:
            return "order does not exist"

    def get_order(self, operation_order_id: str) -> Order:
        res = self.redis_db.hexists(operation_order_id, "Status")
        if res == 1:
            order = Order(
                id=operation_order_id,
                customer_id=self.redis_db.hget(operation_order_id, "Customer ID"),
                target_audience=self.redis_db.hget(operation_order_id, "Target audience"),
                audience_size=int(self.redis_db.hget(operation_order_id,  "Audience size")),
                budget=float(self.redis_db.hget(operation_order_id, "Budget")),
                template_id=self.redis_db.hget(operation_order_id, "Template ID"),
                style_id=self.redis_db.hget(operation_order_id, "Style ID"),
                additional_info=self.redis_db.hget(operation_order_id, "Additional info"),
                status=OrderStatus(int(self.redis_db.hget(operation_order_id, "Status")))
            )

            return order

    def get_all_orders(self) -> list[Order]:
        orders = []
        for key in self.redis_db.scan_iter("*"):
            order = self.get_order(key)
            if order is not None:
                orders.append(self.get_order(key))

        return orders

    def get_multi_order_ids_by_status(self, operation_status: OrderStatus):
        raw_order_ids = self.redis_db.keys()
        result = []
        for order in raw_order_ids:
            if self.redis_db.hexists(order, "Status") == 1:
                if self.redis_db.hget(order, "Status") == str(operation_status):
                    result += order
        return result
