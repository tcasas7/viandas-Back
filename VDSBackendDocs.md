# Viandas del Sur
## Models
### Delivery
Id: **number**

delivered: **boolean**

deliveryDate: **DateTime**

orderId: **number**

Order: **Order**

productId: **number**

Product: **Product**

### Image
Id: **number**

name: **string**

route: **string**

Products: **ICollection[Product]**

### Location
Id: **number**

dir: **string**

isDefault: **boolean**

userId: **number**

User: **User**

### Menu
Id: **number**

category: **string**

price: **number**

validDate: **DateTime**

Products: **ICollection[Product]**

### Order
Id: **number**

price: **double**

paymentMethod: **PaymentMethod**

hasSalt: **boolean**

description: **string**

orderDate: **DateTime**

userId: **number**

location: **string**

User: **User**

Deliveries: **ICollecion[Delivery]**

### Product
Id: **number**

name: **string**

day: **DayOfWeek**

Deliveries: **ICollection[Delivery]**

menuId: **number**

Menu: **Menu**

Image: **Image**

### User
Id: **number**

email: **string**

role: **Role**

salt: **byte[]**

hash: **byte[]**

firstName: **string**

lastName: **string**

phone: **string**

Locations: **ICollection[Location]**

Orders: **ICollection[Order]**

## Enums
### Day
0: **MONDAY**

1: **TUESTDAY**

2: **WEDNESDAY**

3: **THURSDAY**

4: **FRIDAY**

### PaymentMethod
0: **EFECTIVO**

1: **TRANSFERENCIA**

2: **MERCADOPAGO**

### Role
0: **CLIENT**

1: **DELIVERY**

2: **ADMIN**

## Response Models
### Response
statusCode: **number**

message: **string**

### ResponseModel[T]
Inherits: **Response**

model: **T**

### ResponseCollection[T]
Inherits: **Response**

model: **T**

## DTOS
### AddMenusDTO
Menus: **List[MenuDTO]**

### ChangePasswordDTO
email: **string**

phone: **string**

password: **string**

### ChangeRoleDTO
email: **string**

Role: **string**

### DeliveryDTO
Id: **number**

productId: **number**

delivered: **boolean**

deliveryDate: **DayOfWeek**

### LocationDTO
Id: **number**

dir: **string**

isDefault: **boolean**

### LoginDTO
email: **string**

password: **string**

### MenuDTO
Id: **number**

category: **string**

validDate: **DateTime**

price: **double**

### OrderDTO
Id: **number**

price: **double**

paymentMethod: **PaymentMethod**

hasSalt: **boolean**

description: **string**

orderDate: **DateTime**

deliveries: **List[DeliveryDTO]**

### PlaceOrderDTO
Id: **List[OrderDTO]**

### ProductDTO
Id: **number**

day: **DayOfWeek**

name: **string**

### RegisterDTO
email: **string**

password: **string**

firstName: **string**

lastName: **string**

phone: **string**

### UserDTO
Id: **number**

role: **Role**

email: **string**

phone: **string**

firstName: **string**

lastName: **string**

locations: **List[LocationDTO]**

## Controllers

### AuthController
Type: **POST**<br>
Endpoint: **/api/Auth/login**<br>
Body: **LoginDTO**<br>
Header: **null**<br>
Response: **ResponseModel[string]**<br>
Description: **Returns a JWT.**<br>

Type: **GET**<br>
Endpoint: **/api/Auth/health**<br>
Body: **null**<br>
Header: **null**<br>
Response: **Response**<br>
Descriptions: **Checks server status.**<br>

Type: **GET**<br>
Endpoint: **/api/Auth/renew**<br>
Body: **null**<br>
Header: **Authorization**<br>
Response: **ResponseModel[string]**<br>
Description: **Renews the old JWT.**<br>

### MenusController
Type: **GET**<br>
Endpoint: **/api/Menus**<br>
Body: **null**<br>
Header: **null**<br>
Response: **ResponseCollection[MenuDTO]**<br>
Description: **Returns all menus from the DB**<br>

Type: **POST**<br>
Endpoint: **/api/Menus/add**<br>
Body: **AddMenusDTO**<br>
Header: **Authorization**<br>
Response: **Response**<br>
Description: **Saves the given menus in the DB and deletes the old ones.**<br>

Type: **GET**<br>
Endpoint: **/api/Menus/image/$id**<br>
Body: **null**<br>
Header: **null**<br>
Response: **ResponseModel[string]**<br>
Description: **Returns the given productId image.**<br>

Type: **POST**<br>
Endpoint: **/api/Menus/changeImage/$id**<br>
Body: **IFormFile**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Replaces the given productId Image on the DB for the new one.**<br>

## OrdersController
Type: **POST**<br>
Endpoint: **/api/Orders**<br>
Body: **email: string**<br>
Header: **Authorize**<br>
Response: **ResponseCollection[OrderDTO]**<br>
Description: **Returns all the orders of the given User's email.**<br>

Type: **POST**<br>
Endpoint: **/api/Orders/own**<br>
Body: **null**<br>
Header: **Authorize**<br>
Response: **ResponseCollection[OrderDTO]**<br>
Description: **Returns all the orders of the logged User.**<br>

Type: **POST**<br>
Endpoint: **/api/Orders/place**<br>
Body: **PlaceOrderDTO**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Places the given orders saving it into the DB.**<br>

Type: **POST**<br>
Endpoint: **/api/Orders/remove/$orderId**<br>
Body: **null**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Deletes the order of the given orderId.**<br>

## UsersController
Type: **GET**<br>
Endpoint: **/api/Users**<br>
Body: **null**<br>
Header: **Authorize**<br>
Response: **ResponseCollection[UserDTO]**<br>
Description: **Returns all the Users.**<br>

Type: **POST**<br>
Endpoint: **/api/Users/register**<br>
Body: **RegisterDTO**<br>
Header: **null**<br>
Response: **Response**<br>
Description: **Register a new User in the DB with the given information.**<br>

Type: **POST**<br>
Endpoint: **/api/Users/changePassword**<br>
Body: **ChangePasswordDTO**<br>
Header: **null**<br>
Response: **Response**<br>
Description: **Changes the User's password for the new one.**<br>

Type: **POST**<br>
Endpoint: **/api/Users/changeRole**<br>
Body: **ChangeRoleDTO**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Changes the given user Role (Admin use only).**<br>

Type: **POST**<br>
Endpoint: **/api/Users/changePhone/?phone**<br>
Body: **null**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Changes the user's phone number for the given one.**<br>

Type: **GET**<br>
Endpoint: **/api/Users/data**<br>
Body: **null**<br>
Header: **Authorize**<br>
Response: **ResponseModel[UserDTO]**<br>
Description: **Returns the logged User's info.**<br>

Type: **POST**<br>
Endpoint: **/api/Users/addLocation**<br>
Body: **LocationDTO**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Adds the given location to the logged User's info.**<br>

Type: **POST**<br>
Endpoint: **/api/Users/makeDefault**<br>
Body: **LocationDTO**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Replaces the default location for the given one.**<br>

Type: **POST**<br>
Endpoint: **/api/Users/removeLocation**<br>
Body: **LocationDTO**<br>
Header: **Authorize**<br>
Response: **Response**<br>
Description: **Remove the given location from the logged User's info.**<br>
