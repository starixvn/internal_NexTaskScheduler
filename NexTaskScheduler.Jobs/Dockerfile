FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["NexTaskScheduler.Jobs/NexTaskScheduler.Jobs.csproj", "NexTaskScheduler.Jobs/"]
COPY ["NexTaskSheduler.Core/NexTaskScheduler.Core.csproj", "NexTaskSheduler.Core/"]
RUN dotnet restore "NexTaskScheduler.Jobs/NexTaskScheduler.Jobs.csproj"

COPY . .

WORKDIR "/src/NexTaskScheduler.Jobs"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:5.0-buster-slim AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "NexTaskScheduler.Jobs.dll"]