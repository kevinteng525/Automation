json.array!(@automation_tasks) do |automation_task|
  json.extract! automation_task, :name, :status, :task_type, :priority, :create_date, :create_by, :modify_date, :modify_by, :build_id, :supported_environment_id, :test_content, :information, :description
  json.url automation_task_url(automation_task, format: :json)
end
