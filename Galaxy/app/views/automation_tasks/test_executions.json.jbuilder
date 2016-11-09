json.array!(@test_case_executions) do |test_case_execution|
  json.extract! test_case_execution, :test_case_id, :job_id, :status, :start_time, :end_time, :retry_times
  json.url test_case_execution_url(test_case_execution, format: :json)
end