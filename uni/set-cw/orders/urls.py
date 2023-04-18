from django.urls import path
from . import views

urlpatterns = [
    path("orders/", views.orders, name="orders"),
    path("orders/<str:pk>/", views.order, name="order"),
    path("orders/get_customer/<str:pk>/", views.get_customer, name="get_customer"),
]
