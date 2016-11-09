class TestEnvironment < ActiveRecord::Base
  validates :name, :environment_type, :status, :config, :presence => true
  has_many :product_environments, :dependent => :destroy
  ENVIRONMENT_TYPES = ['Public','Private']
  ENVIRONMENT_STATUS = ['Running','Paused','Down','Starting Up', 'Connection Lost']
  attr_accessor :provider_id
  private
  
  def TestEnvironment.get_force_test_environment_by_id(id)
    response = force_server['environments/environments/' + id.to_s].get
    TestEnvironment.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def TestEnvironment.get_all_force_test_environments()
    list = Array.new    
    response = force_server['environments/environments'].get
    params = JSON.parse(response.body)
    params.each do |param|
      list.push TestEnvironment.initialize_from_json_params(param)      
    end
    list
  end
  
  def TestEnvironment.create_force_test_environment(params)
    
  end
  
  def TestEnvironment.update_force_test_environment(id, params)
    
  end
  
  def TestEnvironment.delete_force_test_environment(id)
    
  end
  
  def TestEnvironment.initialize_from_json_params(params)
    environment = TestEnvironment.new
    environment.id = params['EnvironmentId']
    environment.name = params['Name']
    environment.description = params['Description']
    environment.config = params['Config']
    environment.provider_id = params['ProviderId']
    environment.environment_type = params['Type']
    environment.status = params['Status']
    environment
  end
  
  def TestEnvironment.serialize_params_to_json(params)

  end
end
