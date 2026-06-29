# Manual Docker Instructions

- If you wish to run docker manualy instead of through docker-compose, follow these instructions.

1. Create network
```bash
docker network create commercefabric-network
```
2. Run MySQL container
```bash
docker run -d --name mysql-productservice --network commercefabric-network -e MYSQL_ROOT_PASSWORD=admin -e MYSQL_DATABASE=productService -v ./Resources/docker/mysql-init:/docker-entrypoint-initdb.d -p 3306:3306 mysql:9.7.1
```

Note: MySQL must NOT already be running locally on port 3306 unless you change the port mapping.

3. Build microservice image
```bash
docker build -t danielmusselwhite/commercefabric_product_microservice:1.0.0 -f .\CommerceFabric.ProductService\Dockerfile .
```

4. Run microservice
```bash
docker run -p 8080:8080 --network commercefabric-network danielmusselwhite/commercefabric_product_microservice:1.0.0
```

5. Push to Docker Hub
```bash
docker push danielmusselwhite/commercefabric_product_microservice:1.0.0
```