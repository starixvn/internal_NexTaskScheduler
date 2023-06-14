FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["NexTaskScheduler.Jobs/NexTaskScheduler.Jobs.csproj", "NexTaskScheduler.Jobs/"]
COPY ["NexTaskSheduler.Core/NexTaskScheduler.Core.csproj", "NexTaskSheduler.Core/"]
RUN dotnet restore "NexTaskScheduler.Jobs/NexTaskScheduler.Jobs.csproj"

COPY . .

WORKDIR "/src/NexTaskScheduler.Jobs"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "NexTaskScheduler.Jobs.dll"]