module TestResultsHelper
  def result_types_list
    TestResult::RESULT_TYPE.collect{|t|[t,TestResult::RESULT_TYPE.index(t)]}
  end
  def force_executions_list
    TestCaseExecution.get_all_force_test_executions.collect{|e|[e.id,e.id]}
  end
  
    
  def force_test_case_name_by_result_id(id)
    TestResult.get_force_test_case_by_id(id).name
  end
end
