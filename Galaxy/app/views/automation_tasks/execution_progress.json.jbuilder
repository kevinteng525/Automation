json.extract! @task_execution_progress, :task_id, :information, :execution_percentage, :result_type_list, :result_count_list
json.status task_status_name(@task_execution_progress.status)