class SupportedEnvironment < ActiveRecord::Base
  attr_accessor :environment_provider_id
  private
  
  def SupportedEnvironment.get_force_supported_environment_by_id(id)
    response = force_server['environments/templates/' + id.to_s].get
    SupportedEnvironment.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def SupportedEnvironment.get_all_force_supported_environments()
    list = Array.new    
    response = force_server['environments/templates'].get
    params = JSON.parse(response.body)
    params.each do |param|
      list.push SupportedEnvironment.initialize_from_json_params(param)      
    end
    list
  end
  
  def SupportedEnvironment.create_force_supported_environment(params)
    response = force_server['environments/templates'].post SupportedEnvironment.serialize_params_to_json(params),:content_type => 'application/json'
    SupportedEnvironment.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def SupportedEnvironment.update_force_supported_environment(id, params)
    response = force_server['environments/templates/' + id.to_s].put SupportedEnvironment.serialize_params_to_json(params),:content_type => 'application/json'
    SupportedEnvironment.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def SupportedEnvironment.delete_force_supported_environment(id)
    response = force_server['environments/templates/' + id.to_s].delete
  end
  
  def SupportedEnvironment.initialize_from_json_params(params)
    supported_environment = SupportedEnvironment.new
    supported_environment.id = params['EnvironmentId']
    supported_environment.name = params['Name']
    supported_environment.description = params['Description']
    supported_environment.environment_provider_id = params['ProviderId']
    supported_environment.config = params['Config']
    supported_environment
  end
  
  def SupportedEnvironment.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']
    hash[:Description] = params['description']
    hash[:Config] = params['config']
    hash[:ProviderId] = params['environment_provider_id']
    JSON.generate(hash)
  end
  
end
