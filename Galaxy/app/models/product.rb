class Product < ActiveRecord::Base
  has_many :builds, :dependent => :destroy
  has_many :product_environments, :dependent => :destroy
  has_many :test_cases, :dependent => :destroy
  before_destroy :ensure_not_referenced_by_any_items
  attr_accessor :build_provider_id
  attr_accessor :environment_provider_id
  attr_accessor :test_case_provider_id
  attr_accessor :rqm_project_alias

  private
  
  def ensure_not_referenced_by_any_items
    if builds.empty? && product_environments.empty? && test_cases.empty?
      return true
    else
      errors.add(:base, 'Build present.')
      return false
    end
  end
  
  #handle the communication with the Force Server
  def Product.get_force_product_by_id(id)
    response = force_server['products/' + id.to_s].get
    Product.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Product.get_all_force_products()
    list = Array.new    
    response = force_server['products'].get
    params = JSON.parse(response.body)
    params.each do |param|
      list.push Product.initialize_from_json_params(param)      
    end
    list
  end
  
  def Product.create_force_product(params)
   
    response = force_server['products/'].post Product.serialize_params_to_json(params),:content_type => 'application/json', :accept => 'application/json'
    Product.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Product.update_force_product(id, params)
    response = force_server['products/' + id.to_s].put Product.serialize_params_to_json(params),:content_type => 'application/json', :accept=>'application/json'
    Product.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Product.delete_force_product(id)
    response = force_server['products/' + id.to_s].delete
  end
  
  def Product.get_force_builds_for_product(id)
    list = Array.new 
    response = force_server['products/' + id.to_s + '/builds'].get
    if(response != '')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push Build.initialize_from_json_params(param)      
      end
      list      
    else
      nil
    end
  end
  
  def Product.get_force_branches_of_product(id,type)
    list = Array.new
    response = force_server['products/' + id.to_s + "/branches?searchBy=type&type=#{type}"].get
    if(response!='')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push Branch.initialize_from_json_params(param)
      end
      list
    else
      nil
    end
  end

  def Product.get_force_releases_of_product(id,type)
    list = Array.new
    response = force_server['products/' + id.to_s + "/releases?searchBy=type&type=#{type}"].get
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
  
  
  def Product.initialize_from_json_params(params)
    product = Product.new
    product.id = params['ProductId']
    product.build_provider_id = params['BuildProviderId']
    product.environment_provider_id = params['EnvironmentProviderId']
    product.test_case_provider_id = params['TestCaseProviderId']
    product.name = params['Name']
    product.description = params['Description']
    product.rqm_project_alias = params['RQMProjectAlias']
    product
  end
  
  def Product.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']    
    hash[:BuildProviderId] = params['build_provider_id']
    hash[:TestCaseProviderId] = params['test_case_provider_id']
    hash[:EnvironmentProviderId] = params['environment_provider_id']
    hash[:Description] = params['description']
    hash[:RQMProjectAlias] = params['rqm_project_alias']
    JSON.generate(hash)
  end
end
