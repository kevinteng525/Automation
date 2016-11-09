require 'date'
class TestCaseExecution < ActiveRecord::Base
  EXECUTION_STATUS =['NotRunning','Running','Completed','Failed']
  
  private
  
  def TestCaseExecution.get_force_build_by_execution_id(id)
    response = force_server['results/executions/' + id.to_s + '/build'].get
    if response!=''
      Build.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def TestCaseExecution.get_force_test_execution_by_id(id)
    response = force_server['results/executions/' + id.to_s].get
    TestCaseExecution.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestCaseExecution.get_all_force_test_executions()
    list = Array.new    
    response = force_server['results/executions'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        p param
        list.push TestCaseExecution.initialize_from_json_params(param)      
      end
    end
    list
  end
  
  def TestCaseExecution.create_force_test_execution(params)
    response = force_server['results/executions'].post TestCaseExecution.serialize_params_to_json(params),:content_type => 'application/json'
    TestCaseExecution.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestCaseExecution.update_force_test_execution(id, params)
    response = force_server['results/executions/' + id.to_s].put TestCaseExecution.serialize_params_to_json(params),:content_type => 'application/json'
    TestCaseExecution.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestCaseExecution.delete_force_test_execution(id)
    response = force_server['results/executions/' + id.to_s].delete
  end
  
  def TestCaseExecution.initialize_from_json_params(params)
    test_execution = TestCaseExecution.new
    test_execution.id = params['ExecutionId']
    test_execution.test_case_id = params['TestCaseId']
    test_execution.job_id = params['JobId']
    test_execution.status = params['Status']
    test_execution.info = params['Info']
    begin
      test_execution.start_time = Time.parse(db_time_to_rails_time(params['StartTime']))
    rescue
      test_execution.start_time = nil
    end
    begin
      test_execution.end_time = Time.parse(db_time_to_rails_time(params['EndTime']))
    rescue
      test_execution.end_time = nil
    end  
    test_execution.retry_times = params['RetryTimes']
    test_execution

  end
  
  def TestCaseExecution.serialize_params_to_json(params)
    hash = Hash.new
    hash[:TestCaseId] = params['test_case_id']
    hash[:JobId] = params['job_id']
    hash[:Status] = params['status']
    hash[:StartTime] = params['start_time']
    hash[:EndTime] = params['end_time']
    hash[:RetryTimes] = params['retry_times']
    hash[:Info] = params['info']
    JSON.generate(hash)
  end
end
