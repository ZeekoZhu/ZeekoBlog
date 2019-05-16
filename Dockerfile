#FROM zeekozhu/aspnetcore-build-yarn:2.2 AS builder
#WORKDIR /source
#COPY . .
#ENV PATH=${PATH}:/root/.dotnet/tools
#RUN dotnet tool install -g paket
#RUN ./fake.sh build
FROM zeekozhu/aspnetcore-node:2.2-alpine
ENV ASPNETCORE_ENVIRONMENT $APPENV
WORKDIR /app
COPY ./ZeekoBlog/publish .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]
