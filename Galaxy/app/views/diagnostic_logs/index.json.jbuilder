json.array!(@diagnostic_logs) do |diagnostic_log|
  json.extract! diagnostic_log, :create_time, :component, :log_type, :message
  json.url diagnostic_log_url(diagnostic_log, format: :json)
end
