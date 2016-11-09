class Provider < ActiveRecord::Base
  private
  def Provider.initialize_from_json_params(params)
    provider = Provider.new    
    provider.id = params['ProviderId']
    provider.name = params['Name']
    provider.category = params['Category']
    provider.type = params['Type']
    provider.path = params['Path']
    provider.config = params['Config']
    provider.description = params['Description']
    provider.is_active = params['IsActive']
    provider
  end
  def Provider.get_provider_list_by_category(category) 
    list = Array.new 
    response = force_server["Providers?searchBy=category&category=#{category}"].get    
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push Provider.initialize_from_json_params(param)
      end
    end
    list
  end

end
