json.array!(@automation_jobs) do |job|
  json.extract! job, :id, :name, :sut_environment_id, :test_agent_environment_id, :job_type, :priority, :status, :retry_times, :time_out, :description, :created_at, :updated_at
  json.url automation_job_url(job, format: :json)
end