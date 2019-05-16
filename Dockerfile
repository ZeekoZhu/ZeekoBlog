FROM zeekozhu/aspnetcore-build-yarn:2.2 AS builder
WORKDIR /source
COPY . .
ENV PATH=${PATH}:/root/.dotnet/tools
ENV ASPNETCORE_ENVIRONMENT $APPENV
RUN ./fake.sh build
FROM zeekozhu/aspnetcore-node:2.2-alpine
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]
