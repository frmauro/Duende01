## Build the Docker image
docker build -t duendeidentityserver .

## Run the Docker container
docker run -d -p 8080:80 --name myidentityserver duendeidentityserver

## Run the Docker container with a custom connection string
docker run -d -p 8080:80 --name myidentityserver -e ConnectionStrings__DefaultConnection="Data Source=Duende.IdentityServer.Quickstart.EntityFramework.db" duendeidentityserver
