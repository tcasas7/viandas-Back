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

userId: **number**
User: **User**

### Menu
Id: **number**
category: **string**
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

### LoginDTO
email: **string**
password: **string**

### MenuDTO
Id: **number**
category: **string**
validDate: **DateTime**

### OrderDTO
Id: **number**
price: **double**
paymentMethod: **PaymentMethod**
hasSalt: **boolean**
description: **string**
orderDate: **DateTime**

deliveries: **List[DeliveryDTO]**

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
Type: **POST**
Endpoint: **/api/Auth/login**
Body: **LoginDTO**
Header: **null**
Response: **ResponseModel[string]**
Description: **Returns a JWT.**

Type: **GET**
Endpoint: **/api/Auth/health**
Body: **null**
Header: **null**
Response: **Response**
Descriptions: **Checks server status.**

Type: **GET**
Endpoint: **/api/Auth/renew**
Body: **null**
Header: **Authorization**
Response: **ResponseModel[string]**
Description: **Renews the old JWT.**

### MenusController
Type: **GET**
Endpoint: **/api/Menus**
Body: **null**
Header: **null**
Response: **ResponseCollection[MenuDTO]**
Description: **Returns all menus from the DB**

Type: **POST**
Endpoint: **/api/Menus/add**
Body: **AddMenusDTO**
Header: **Authorization**
Response: **Response**
Description: **Saves the given menus in the DB and deletes the old ones.**

Type: **GET**
Endpoint: **/api/Menus/image/$id**
Body: **null**
Header: **null**
Response: **ResponseModel[string]**
Description: **Returns the given productId image.**

Type: **POST**
Endpoint: **/api/Menus/changeImage/$id**
Body: **IFormFile**
Header: **Authorize**
Response: **Response**
Description: **Replaces the given productId Image on the DB for the new one.**

## OrdersController
Type: **POST**
Endpoint: **/api/Orders**
Body: **email: string**
Header: **Authorize**
Response: **ResponseCollection[OrderDTO]**
Description: **Returns all the orders of the given User's email.**

Type: **POST**
Endpoint: **/api/Orders/own**
Body: **null**
Header: **Authorize**
Response: **ResponseCollection[OrderDTO]**
Description: **Returns all the orders of the logged User.**

Type: **POST**
Endpoint: **/api/Orders/place**
Body: **OrderDTO**
Header: **Authorize**
Response: **Response**
Description: **Places the given order saving it into the DB.**

Type: **POST**
Endpoint: **/api/Orders/remove/$orderId**
Body: **null**
Header: **Authorize**
Response: **Response**
Description: **Deletes the order of the given orderId.**

## UsersController
Type: **GET**
Endpoint: **/api/Users**
Body: **null**
Header: **Authorize**
Response: **ResponseCollection[UserDTO]**
Description: **Returns all the Users.**

Type: **POST**
Endpoint: **/api/Users/register**
Body: **RegisterDTO**
Header: **null**
Response: **Response**
Description: **Register a new User in the DB with the given information.**

Type: **POST**
Endpoint: **/api/Users/changePassword**
Body: **ChangePasswordDTO**
Header: **null**
Response: **Response**
Description: **Changes the User's password for the new one.**

Type: **POST**
Endpoint: **/api/Users/changeRole**
Body: **ChangeRoleDTO**
Header: **Authorize**
Response: **Response**
Description: **Changes the given user Role (Admin use only).**

Type: **GET**
Endpoint: **/api/Users/data**
Body: **null**
Header: **Authorize**
Response: **ResponseModel[UserDTO]**
Description: **Returns the logged User's info.**

Type: **POST**
Endpoint: **/api/Users/addLocation**
Body: **LocationDTO**
Header: **Authorize**
Response: **Response**
Description: **Adds the given location to the logged User's info.**

Type: **POST**
Endpoint: **/api/Users/removeLocation**
Body: **LocationDTO**
Header: **Authorize**
Response: **Response**
Description: **Remove the given location from the logged User's info.**
