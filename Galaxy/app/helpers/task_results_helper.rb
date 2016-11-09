module TaskResultsHelper    
  def force_task_name(id)
    AutomationTask.get_force_automation_task_by_id(id).name
  end
  def force_result_by_execution_id(id)
    TestResult.get_force_test_result_by_execution_id(id)
  end
  def force_build_by_execution_id(id)
    TestCaseExecution.get_force_build_by_execution_id(id)
  end

end
