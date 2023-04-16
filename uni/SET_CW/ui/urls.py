from django.urls import path
from . import views

urlpatterns = [
    path("orders/", views.orders, name="orders"),
    path("orders/<str:pk>/", views.order, name="order"),
]
