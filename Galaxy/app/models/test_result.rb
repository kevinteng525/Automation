class HistoricalResult
  attr_accessor :build_id, :build_name, :task_id, :task_name, :test_case_id, :test_case_name, :result_id, :result, :result_comments
end

class TestResult < ActiveRecord::Base

RESULT_TYPE=['Pass','Failed','Time Out','Exception','Not Run', 'Known Product Issue','New Product Issue','Environment Issue', 'Scripts Issue', 'Common Library Issue']
private
#compare the result of two tasks.
  def TestResult.get_force_test_result_comparation_of_two_task(task1_id, task2_id)
    array_of_execution_array = Array.new
    response = force_server['results/taskresultcompare/' + task1_id + ',' + task2_id].get
    params = JSON.parse(response.body)
    params.each do |param|
      execution_array = Array.new
      param.each do |execution_param|
        execution_array.push TestCaseExecution.initialize_from_json_params(execution_param)
      end
      array_of_execution_array.push execution_array
    end
    array_of_execution_array
  end

  def TestResult.get_force_test_result_by_id(id)
    response = force_server['results/results/' + id.to_s].get
    TestResult.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestResult.get_force_test_case_by_id(id)
    response = force_server['results/results/' + id.to_s + '/testcase'].get
    TestCase.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestResult.get_force_test_result_by_execution_id(id)
    response = force_server['results/executions/' + id.to_s + '/result'].get
    if response!=''
      TestResult.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def TestResult.get_force_historical_test_results_by_testcase_id(id)
    response = force_server['testdepot/testcases/' + id.to_s + '/historicaltestresults?size=5'].get
    historical_results = Array.new
    if response.body!=''
      params = JSON.parse(response.body)
      params.each do |param|
        historical_result = TestResult.initialize_historical_result_from_json_params(param)
        historical_results.push historical_result
      end
    end
    historical_results
  end
  
  def TestResult.get_all_force_test_results()
    list = Array.new    
    response = force_server['results/results'].get
    if response.body!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestResult.initialize_from_json_params(param)      
      end
    end
    list
  end
  
  def TestResult.create_force_test_result(params)
    response = force_server['results/results'].post TestResult.serialize_params_to_json(params),:content_type => 'application/json'
    TestResult.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestResult.update_force_test_result(id, params)
    response = force_server['results/results/' + id.to_s].put TestResult.serialize_params_to_json(params),:content_type => 'application/json'
    TestResult.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestResult.delete_force_test_result(id)
    response = force_server['results/results/' + id.to_s].delete
  end
  
  def TestResult.initialize_from_json_params(params)
    test_result = TestResult.new
    test_result.id = params['ResultId']
    test_result.execution_id = params['ExecutionId']
    test_result.result = params['Result']
    test_result.is_triaged = params['IsTriaged']
    test_result.triaged_by = params['TriagedBy']
    test_result.description = params['Description']
    test_result.files = params['Files']
    test_result
  end
  
  def TestResult.initialize_historical_result_from_json_params(params)
    historical_result = HistoricalResult.new
    historical_result.build_id = params['BuildId']
    historical_result.build_name = params['BuildName']
    historical_result.task_id = params['TaskId']
    historical_result.task_name = params['TaskName']
    historical_result.test_case_id = params['TestCaseId']
    historical_result.test_case_name = params['TestCaseName']
    historical_result.result_id = params['ResultId']
    historical_result.result = params['Result']
    historical_result.result_comments = params['ResultComments']
    historical_result
  end
  
  def TestResult.serialize_params_to_json(params)
    hash = Hash.new
    hash[:ExecutionId] = params['execution_id']
    hash[:Result] = params['result']
    hash[:IsTriaged] = params['is_triaged']
    hash[:TriagedBy] = params['triaged_by']
    hash[:Description] = params['description']
    hash[:Files] = params['files']
    JSON.generate(hash)
  end
end
