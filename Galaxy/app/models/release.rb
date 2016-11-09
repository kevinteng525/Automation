class Release < ActiveRecord::Base
  def Release.get_all_force_releases
    list = Array.new
    response = force_server['galaxyglobal/release'].get
    if(response!='')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push Release.initialize_from_json_params(param)
      end
      list
    else
      nil
    end
  end
  def Release.get_all_force_releases_by_type(type)
    list = Array.new
    response = force_server["galaxyglobal/release?searchBy=type&type=#{type}"].get
    if(response!='')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push Release.initialize_from_json_params(param)
      end
      list
    else
      nil
    end
  end
  private
  def Release.initialize_from_json_params(params)
    release = Release.new    
    release.id = params['ReleaseId']
    release.name = params['Name']
    release.description = params['Description']
    release.product_id = params['ProductId']
    release.path = params['Path']
    release
  end
  def Release.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']    
    hash[:Description] = params['description']
    hash[:ProductId] = params['product_id']  
    hash[:Path] = params['path']
    JSON.generate(hash)
  end
end
