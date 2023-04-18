from django.urls import path
from . import views

urlpatterns = [
    path("templates/", views.templates, name="templates"),
    path("templates/<str:pk>/", views.template, name="template"),
    path("htmls/", views.htmls, name="htmls"),
    path("htmls/<str:pk>/", views.html, name="html"),
    path("styles/", views.styles, name="styles"),
    path("styles/<str:pk>/", views.style, name="style"),
    path("formats/", views.get_formats, name="formats"),
    path("partners/", views.partners, name="partners"),
    path("partners/<str:pk>/", views.partner, name="partner"),
    path("fields_of_service/", views.fields_of_service, name="fields_of_service"),
    path("fields_of_service/<str:pk>/", views.field_of_service, name="field_of_service"),
    path("connections/", views.connections, name="connections"),
]
