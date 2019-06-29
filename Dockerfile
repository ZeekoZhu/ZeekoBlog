FROM zeekozhu/aspnetcore-node:2.2-alpine
ENV ASPNETCORE_ENVIRONMENT $APPENV
WORKDIR /app
COPY ./ZeekoBlog/artifacts .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]
