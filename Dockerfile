FROM zeekozhu/aspnetcore-node:3.0-alpine
ENV ASPNETCORE_ENVIRONMENT $APPENV
WORKDIR /app
COPY ./ZeekoBlog/artifacts .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]
