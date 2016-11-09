class Build < ActiveRecord::Base
  belongs_to :product
  validates :product_id, :name, :build_type, :status, :branch, :number, :location, :presence => true
  attr_accessor :release
  
  BUILD_TYPES = ['Official','Private']
  BUILD_STATUS = ['Not Exist','Success','Failed', 'Delete']
  
  private
  
  def Build.get_force_build_by_id(id)
    response = force_server['builds/' + id.to_s].get
    Build.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Build.get_all_force_builds()
    list = Array.new    
    response = force_server['builds'].get
    params = JSON.parse(response.body)
    params.each do |param|
      list.push Build.initialize_from_json_params(param)      
    end
    list
  end
  
  def Build.create_force_build(params)
    response = force_server['builds/'].post Build.serialize_params_to_json(params),:content_type => 'application/json'
    Build.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Build.update_force_build(id, params)
    response = force_server['builds/' + id.to_s].put Build.serialize_params_to_json(params),:content_type => 'application/json'
    Build.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Build.delete_force_build(id)
    response = force_server['builds/' + id.to_s].delete
  end
  
  def Build.initialize_from_json_params(params)
    build = Build.new
    build.id = params['BuildId']
    build.product_id = params['ProductId']
    build.name = params['Name']
    build.build_type = params['Type']
    build.status = params['Status']
    build.branch = params['Branch']
    build.number = params['Number']
    build.location = params['Location']
    build.description = params['Description']
    build.release = params['Release']
    build
  end
  
  def Build.serialize_params_to_json(params)
    hash = Hash.new
    hash[:ProductId] = params['product_id']
    hash[:Name] = params['name']
    hash[:Type] = params['build_type']
    hash[:Status] = params['status']
    hash[:Branch] = params['branch']
    hash[:Number] = params['number']
    hash[:Location] = params['location']
    hash[:Description] = params['description']
    hash[:Release] = params['release']
    JSON.generate(hash)
  end
  
end
