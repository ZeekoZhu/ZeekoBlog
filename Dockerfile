FROM zeekozhu/aspnetcore-build-yarn:2.1 AS builder
WORKDIR /source
COPY . .
RUN ./fake.sh build
FROM zeekozhu/aspnetcore-node:2.1-alpine
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]
