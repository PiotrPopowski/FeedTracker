# FeedTracker

Simple .NET app in microservice architecture built for fun. It fetches data from opensource API (Feeds) and streams it to Aggregator, which publishes metrics (with Prometheus) for showing it on Grafana dashboard and ocasionally sends messages via message broaker to Notifier, which then fetches required data using gRPC from Users microservice and notify the users that certain event occured.

## Technologies

- **ASP.NET 8**
- **Redis**
- **Prometheus**
- **Grafana**
- **gRPC**
- **Quartz**
- **Docker**
