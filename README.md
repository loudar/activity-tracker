# Query data

```
from(bucket: "test-bucket")
  |> range(start: -15m) // Adjust the time range as needed
  |> filter(fn: (r) => r._measurement == "active_app")
  |> filter(fn: (r) => r._field == "window_title")
  |> group(columns: ["process_name"])
  |> aggregateWindow(every: 1m, fn: count) // Adjust the window aggregation as needed
  |> yield(name: "count")
```