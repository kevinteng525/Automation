module TestCaseExecutionsHelper
  def force_jobs_list
    
  end
  
  def execution_status_list
    TestCaseExecution::EXECUTION_STATUS.collect{|s|[s,TestCaseExecution::EXECUTION_STATUS.index(s)]}
  end
  
  def execution_status_name(id)
    TestCaseExecution::EXECUTION_STATUS[id]
  end
end
