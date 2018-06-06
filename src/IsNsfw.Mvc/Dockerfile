FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime

COPY ./obj/Docker/publish/ /publish
WORKDIR /publish
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "IsNsfw.Mvc.dll"]