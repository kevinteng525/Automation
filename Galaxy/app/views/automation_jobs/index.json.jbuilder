json.array!(@automation_jobs) do |automation_job|
  json.extract! automation_job, :name, :sut_environment_id, :test_agent_environment_id, :job_type, :priority, :status, :retry_times, :time_out, :create_by, :modify_by, :description
  json.url automation_job_url(automation_job, format: :json)
end
