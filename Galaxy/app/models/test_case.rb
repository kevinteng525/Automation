class TestCase < ActiveRecord::Base
  belongs_to :product
  SOURCE_LIST=['RQM','TFS']
  
  private
  
  def TestCase.get_test_case_rankings()
    response = force_server['testdepot/rankings'].get
    if response.body != ''
    JSON.parse(response.body)
    else
      nil
    end
  end
  
    def TestCase.get_test_case_releases()
    response = force_server['testdepot/releases'].get
    if response.body != ''
    JSON.parse(response.body)
    else
      nil
    end
  end
  
  def TestCase.get_force_test_case_by_id(id)
    response = force_server['testdepot/testcases/' + id.to_s].get
    TestCase.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestCase.get_all_force_test_cases()
    list = Array.new    
    response = force_server['testdepot/testcases'].get
    if response.body != ''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestCase.initialize_from_json_params(param)      
      end
    end
    list
  end
  
  def TestCase.create_force_test_case(params)
    response = force_server['testdepot/testcases'].post TestCase.serialize_params_to_json(params),:content_type => 'application/json'
    TestCase.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestCase.update_force_test_case(id, params)
    response = force_server['testdepot/testcases/' + id.to_s].put TestCase.serialize_params_to_json(params),:content_type => 'application/json'
    TestCase.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestCase.delete_force_test_case(id)
    response = force_server['testdepot/testcases/' + id.to_s].delete
  end
  
  def TestCase.get_sub_test_cases_of_test_suite(id)
    list = Array.new    
    response = force_server['testdepot/testsuites/' + id.to_s + '/subtestcases'].get
    if(response!='')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestCase.initialize_from_json_params(param)      
      end
      list
    else
      nil
    end
  end
  
  def TestCase.initialize_from_json_params(params)
    test_case = TestCase.new
    test_case.id = params['TestCaseId']    
    test_case.source_id = params['SourceId'] 
    test_case.name = params['Name'] 
    test_case.product_id = params['ProductId'] 
    test_case.feature = params['Feature'] 
    test_case.script_path = params['ScriptPath'] 
    test_case.weight = params['Weight'] 
    test_case.is_automated = params['IsAutomated'] 
    test_case.created_by = params['CreateBy']
    test_case.created_at = ms_json_to_date(params['CreateTime'])
    test_case.modified_by = params['ModifyBy']
    test_case.updated_at = ms_json_to_date(params['ModifyTime'])
    test_case.description = params['Description'] 
    test_case
  end
  
  def TestCase.serialize_params_to_json(params)
    hash = Hash.new
    hash[:SourceId] = params['source_id']
    hash[:Name] = params['name']
    hash[:ProductId] = params['product_id']
    hash[:Feature] = params['feature']
    hash[:ScriptPath] = params['script_path']
    hash[:Weight] = params['weight']
    hash[:IsAutomated] = params['is_automated']
    hash[:CreateBy] = params['created_by']
    hash[:CreateTime] = params['created_at']
    hash[:ModifyBy] = params['modified_by']
    hash[:ModifyTime] = params['updated_at']
    hash[:Description] = params['description']
    
    JSON.generate(hash)
  end
  
end
