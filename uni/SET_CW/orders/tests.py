from django.test import TestCase
from . import redis
from .models import *


class RedisTests(TestCase):
    def setUp(self) -> None:
        db = redis.OrdersDB()
        for key in db.redis_db.scan_iter():
            db.redis_db.delete(key)

    def test_add_order(self) -> None:
        db = redis.OrdersDB()
        pk = db.add_order(Order(id="", customer_id="", target_audience="", audience_size=0, budget=0.0,
                                template_id="", style_id="", additional_info="", status=OrderStatus.Created))

        self.assertIsNotNone(db.redis_db.hexists(pk, "Status"))
        self.assertEqual("", db.redis_db.hget(pk, "Customer ID"), "Customer ID does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Target audience"), "Target audience does not match")
        self.assertEqual(0, int(db.redis_db.hget(pk, "Audience size")), "Audience size does not match")
        self.assertEqual(0.0, float(db.redis_db.hget(pk, "Budget")), "Budget does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Template ID"), "Template ID does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Style ID"), "Style ID does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Additional info"), "Additional info does not match")
        self.assertEqual(OrderStatus.Created, OrderStatus(int(db.redis_db.hget(pk, "Status"))),
                         "Status does not match")

    def test_change_order(self):
        db = redis.OrdersDB()
        pk = db.add_order(Order(id="", customer_id="", target_audience="", audience_size=0, budget=0.0,
                                template_id="", style_id="", additional_info="", status=OrderStatus.Created))
        db.change_order(Order(id=pk, customer_id="", target_audience="", audience_size=1, budget=0.0,
                              template_id="", style_id="", additional_info="", status=OrderStatus.Created))

        self.assertIsNotNone(db.redis_db.hexists(pk, "Status"))
        self.assertEqual("", db.redis_db.hget(pk, "Customer ID"), "Customer ID does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Target audience"), "Target audience does not match")
        self.assertEqual(1, int(db.redis_db.hget(pk, "Audience size")), "Audience size should have changed")
        self.assertEqual(0.0, float(db.redis_db.hget(pk, "Budget")), "Budget does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Template ID"), "Template ID does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Style ID"), "Style ID does not match")
        self.assertEqual("", db.redis_db.hget(pk, "Additional info"), "Additional info does not match")
        self.assertEqual(OrderStatus.Created, OrderStatus(int(db.redis_db.hget(pk, "Status"))),
                         "Status does not match")

    def test_delete_order(self):
        db = redis.OrdersDB()
        pk = db.add_order(Order(id="", customer_id="", target_audience="", audience_size=0, budget=0.0,
                                template_id="", style_id="", additional_info="", status=OrderStatus.Created))
        result = db.delete_order(pk)

        self.assertEqual("order deleted", result, "Order should have been deleted")
        count = 0
        for key in db.redis_db.scan_iter():
            if db.redis_db.hget("*", key) is not None:
                count += 1
        self.assertEqual(0, count, "db should contain 0 orders")

    def test_get_all_orders(self):
        db = redis.OrdersDB()
        for i in range(4):
            db.add_order(Order(id="", customer_id="", target_audience="", audience_size=i, budget=0.0,
                               template_id="", style_id="", additional_info="", status=OrderStatus.Created))

        result = db.get_all_orders()

        self.assertEqual(len(result), 4, "Should fetch 4 orders")

    def test_get_order(self):
        db = redis.OrdersDB()
        _ = db.add_order(Order(id="", customer_id="", target_audience="", audience_size=0, budget=0.0,
                               template_id="", style_id="", additional_info="", status=OrderStatus.Created))
        pk2 = db.add_order(Order(id="", customer_id="", target_audience="", audience_size=1, budget=0.0,
                                 template_id="", style_id="", additional_info="", status=OrderStatus.Created))

        result = db.get_order(pk2)

        self.assertIsNotNone(result, "Should not be None")
        self.assertEqual(result.audience_size, 1, "Audience size of the fetched order should be 1")
        self.assertEqual(result.id, pk2, "ID of the fetched order does not match")
