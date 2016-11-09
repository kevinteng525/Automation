class TestSuite < ActiveRecord::Base
  SUITE_TYPES = ['Normal','Internal']
  attr_accessor :execution_command
  private
  
   def TestSuite.refresh_test_case_from_external_system()
    begin
      response = force_server['testdepot/testcases/refresh'].post :content_type => 'application/json', :timeout => 300
      if response.body != ''
        true
      else
        nil
      end
    rescue
      nil
    end    
  end

  def TestSuite.refresh_test_suite_from_external_system()
    begin
      response = force_server['testdepot/testsuites/refresh'].post :content_type => 'application/json', :timeout => 300
      if response.body != ''
        true
      else
        nil
      end
    rescue
      nil
    end    
  end
  
  def TestSuite.get_root_test_suite()
    response = force_server['testdepot/root'].get
    if response.body != ''
      TestSuite.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def TestSuite.get_root_user_created_test_suite()
    response = force_server['testdepot/usercreatedsuitesroot'].get
    if response.body != ''
      TestSuite.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def TestSuite.get_root_rqm_test_suite()
    response = force_server['testdepot/rqmsuitesroot'].get
    if response.body != ''
      TestSuite.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def TestSuite.get_root_rqm_test_plans_suite()
    response = force_server['testdepot/rqmtestplansroot'].get
    if response.body != ''
      TestSuite.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def TestSuite.get_force_test_suite_by_id(id)
    response = force_server['testdepot/testsuites/' + id.to_s].get
    TestSuite.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestSuite.get_all_force_test_suites()
    list = Array.new    
    response = force_server['testdepot/testsuites'].get
    if response.body!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestSuite.initialize_from_json_params(param)      
      end
    end
    list
  end
  
  def TestSuite.create_force_test_suite(params)
    response = force_server['testdepot/testsuites'].post TestSuite.serialize_params_to_json(params),:content_type => 'application/json'
    TestSuite.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestSuite.update_force_test_suite(id, params)
    response = force_server['testdepot/testsuites/' + id.to_s].put TestSuite.serialize_params_to_json(params),:content_type => 'application/json'
    TestSuite.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestSuite.delete_force_test_suite(id)
    response = force_server['testdepot/testsuites/' + id.to_s].delete
  end
  
  def TestSuite.get_sub_test_suites_of_test_suite(id)
    list = Array.new    
    response = force_server['testdepot/testsuites/' + id.to_s + '/subtestsuites'].get
    if(response!='')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestSuite.initialize_from_json_params(param)      
      end
      list
    else
      nil
    end
  end
  
  def TestSuite.initialize_from_json_params(params)
    test_suite = TestSuite.new
    test_suite.id = params['SuiteId']
    test_suite.name = params['Name']
    test_suite.sub_suites = params['SubSuites']
    test_suite.test_cases = params['TestCases']
    test_suite.created_by = params['CreateBy']
    test_suite.created_at = ms_json_to_date(params['CreateTime'])
    test_suite.modified_by = params['ModityBy']
    test_suite.updated_at = ms_json_to_date(params['ModifyTime'])
    test_suite.description = params['Description']
    test_suite.suite_type = params['Type'] 
    test_suite.execution_command = params['ExecutionCommand']   
    test_suite
  end
  
  def TestSuite.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']
    hash[:SubSuites] = params['sub_suites']
    hash[:TestCases] = params['test_cases']
    hash[:CreateBy] = params['created_by']
    hash[:CreateTime] = params['created_at']
    hash[:ModityBy] = params['modified_by']
    hash[:ModifyTime] = params['updated_at']
    hash[:Description] = params['description']
    hash[:Type] = params['suite_type']
    hash[:ExecutionCommand] = params['execution_command']
    JSON.generate(hash)
  end
  
end
