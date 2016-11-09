class ProductSupportedEnvironmentMap < ActiveRecord::Base
  attr_accessor :project_id
  private
  
  def ProductSupportedEnvironmentMap.get_force_supported_environment_product_map_by_id(id)
    response = force_server['environments/ProjectsTemplatesMap/' + id.to_s].get
    ProductSupportedEnvironmentMap.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def ProductSupportedEnvironmentMap.get_all_force_supported_environment_product_maps()
    list = Array.new    
    response = force_server['environments/ProjectsTemplatesMap'].get
    params = JSON.parse(response.body)
    params.each do |param|
      list.push ProductSupportedEnvironmentMap.initialize_from_json_params(param)      
    end
    list
  end
  
  def ProductSupportedEnvironmentMap.create_force_supported_environment_product_map(params)
    response = force_server['environments/ProjectsTemplatesMap'].post ProductSupportedEnvironmentMap.serialize_params_to_json(params),:content_type => 'application/json'
    ProductSupportedEnvironmentMap.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def ProductSupportedEnvironmentMap.update_force_supported_environment_product_map(id, params)
    response = force_server['environments/ProjectsTemplatesMap/' + id.to_s].put ProductSupportedEnvironmentMap.serialize_params_to_json(params),:content_type => 'application/json'
    ProductSupportedEnvironmentMap.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def ProductSupportedEnvironmentMap.delete_force_supported_environment_product_map(id)
    response = force_server['environments/ProjectsTemplatesMap/' + id.to_s].delete
  end
  
  def ProductSupportedEnvironmentMap.initialize_from_json_params(params)
    map = ProductSupportedEnvironmentMap.new
    map.id = params['MapId']
    map.project_id = params['ProjectId']
    map.supported_environment_id = params['EnvironmentId']
    map
  end
  
  def ProductSupportedEnvironmentMap.serialize_params_to_json(params)
    hash = Hash.new
    hash[:ProjectId] = params['project_id']
    hash[:EnvironmentId] = params['supported_environment_id']
    JSON.generate(hash)
  end
  
end
