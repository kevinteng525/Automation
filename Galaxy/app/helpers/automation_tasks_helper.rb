module AutomationTasksHelper
  def task_type_list
    AutomationTask::AUTOMATION_TASK_TYPES.collect{|t|[t,AutomationTask::AUTOMATION_TASK_TYPES.index(t)]}
  end
  def task_status_list
    AutomationTask::AUTOMATION_TASK_STATUS.collect{|t|[t,AutomationTask::AUTOMATION_TASK_STATUS.index(t)]}
  end
  def task_priority_list
    AutomationTask::AUTOMATION_TASK_PRIORITIES.collect{|t|[t,AutomationTask::AUTOMATION_TASK_PRIORITIES.index(t)]}
  end
  
  def task_schedule_pattern_list
    AutomationTask::AUTOMATION_TASK_SCHEDULE_PATTERN.collect{|t|[t, AutomationTask::AUTOMATION_TASK_SCHEDULE_PATTERN.index(t)]}
  end
  
  def task_type_name(index)
    if(index)
      AutomationTask::AUTOMATION_TASK_TYPES[index]
    else
      ''
    end
  end
  
  def task_status_name(index)
    if(index)
      AutomationTask::AUTOMATION_TASK_STATUS[index]
    else
      ''
    end
  end
  
  def task_priority_name(index)
    if(index)
      AutomationTask::AUTOMATION_TASK_PRIORITIES[index]
    else
      ''
    end 
  end

  
  def automation_task_completed(status)
    case task_status_name(status)
    when 'Complete'
      return true
    when 'Cancelled'
      return false
    when 'End'
      return true
    else
      return false
    end
  end
  
  def force_builds_list
    Build.get_all_force_builds.collect{|b|[b.name, b.id]}
  end

  def force_branches_list
    Branch.get_force_branches_by_type(0).collect{|b|[b.name, b.id]}
  end

  def force_releases_list
    Release.get_all_force_releases_by_type(0).collect{|b|[b.name, b.id]}
  end
  
  def force_supported_environments_list
    SupportedEnvironment.get_all_force_supported_environments().collect{|b|[b.name, b.id]}
  end
  
  def task_schedule_recurrence_pattern_name(index)
    AutomationTask::AUTOMATION_TASK_SCHEDULE_PATTERN[index]
  end
  
  def force_test_result_of_test_execution(execution)
    result = TestResult.get_force_test_result_by_execution_id(execution.id)
    if(result == nil)
      "Not available"
    else
      TestResult::RESULT_TYPE[result.result]
    end
  end
    
  
end
