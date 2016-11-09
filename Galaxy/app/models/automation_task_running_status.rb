class AutomationTaskRunningStatus < ActiveRecord::Base
  
  def AutomationTaskRunningStatus.initialize_from_json_params(params)
    automation_task_running_status = AutomationTaskRunningStatus.new
    automation_task_running_status.task_id = params['TaskId']
    automation_task_running_status.status = params['Status']
    automation_task_running_status.result_type_list = params['TestCasesExecutionStatusList']
    automation_task_running_status.result_count_list = params['TestCasesExecutionStatusCountList']
    automation_task_running_status.execution_percentage = params['ExecutionPercentage']
    automation_task_running_status.information = params['Information']
    automation_task_running_status
  end
  
end
