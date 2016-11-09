module TestEnvironmentsHelper
  
  def get_environment_type_list
    TestEnvironment::ENVIRONMENT_TYPES.collect{|te|[te,te]}
  end
  
  def get_environment_status_list
    TestEnvironment::ENVIRONMENT_STATUS.collect{|ts|[ts,ts]}
  end
  
  def get_force_environment_name_by_id(id)
    environment = TestEnvironment.get_force_test_environment_by_id(id)
    if environment!=nil
      environment.name
    else
      ''
    end
  end
end
