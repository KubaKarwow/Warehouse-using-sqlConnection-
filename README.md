# Warehouse API

Using this database scheme
![image](https://github.com/user-attachments/assets/d18525fc-2a95-4866-b0bd-98d6c2f4d335)


This API allows to use these endpoints:

-Add Product to Warehouse Endpoint
-
This endpoint allows adding a product to the warehouse under specified conditions:

Check Product and Warehouse Existence: Verify if the product with the given identifier exists and whether the warehouse with the specified identifier exists.

Validate Quantity: Ensure that the quantity provided in the request is greater than 0.

Check Purchase Order: Products can only be added to the warehouse if there is a corresponding purchase order in the Order table. Verify if there exists a record in the Order table matching the ProductId and Amount specified in the request. The order creation date (Order.CreatedAt) should be earlier than the current request's creation date.

Verify Order Fulfillment: Ensure that the order has not already been fulfilled by checking for the absence of a record with the corresponding OrderId in the Product_Warehouse table.

Update Order Fulfillment: Update the FullfilledAt column of the order to the current date and time (UPDATE operation).

Insert Record into Product_Warehouse: Insert a new record into the Product_Warehouse table. The Price column should reflect the product price multiplied by the Amount from the order. Additionally, set the CreatedAt value to the current timestamp (INSERT operation).

Return Primary Key: Return the primary key value generated for the inserted record in the Product_Warehouse table.

Endpoint: /api/warehouse/products
Method: POST

-Retrieve Warehouse Product Endpoint
-
This endpoint retrieves details of a specific product stored in the warehouse, including associated purchase orders and fulfillment status.

Retrieve Product Details: Fetch details of the product based on the provided ProductId.

Retrieve Purchase Orders: Retrieve all purchase orders (Order records) associated with the product. Ensure these orders are sorted by creation date (Order.CreatedAt).

Fetch Fulfillment Status: Check the fulfillment status of each order. Verify if there exists a corresponding record in the Product_Warehouse table for each order (Product_Warehouse.OrderId).

Endpoint: /api/warehouse/products/{productId}
Method: GET


