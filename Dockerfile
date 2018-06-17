FROM zeekzhu/aspnetcore-build-yarn:2.1 AS builder
WORKDIR /source
COPY ./ZeekoBlog/*.csproj ./ZeekoBlog/
COPY ./ZeekoBlog/*.json ./ZeekoBlog/
COPY ./Nuget.Config .
ARG APPENV
ENV ASPNETCORE_ENVIRONMENT ${APPENV}
RUN cd ZeekoBlog && dotnet restore --configfile ../Nuget.Config && npm install
COPY . .
RUN cd ZeekoBlog && npm run build && dotnet publish -c Release -o /app/
FROM zeekozhu/aspnetcore-node:2.1-alpine
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "ZeekoBlog.dll"]
