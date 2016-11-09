class Branch < ActiveRecord::Base
  def Branch.get_all_force_branches
    list = Array.new
    response = force_server['galaxyglobal/branch'].get
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
  def Branch.get_force_branches_by_type(type)
    list = Array.new
    response = force_server["galaxyglobal/branch?searchBy=type&type=#{type}"].get
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
  private
  def Branch.initialize_from_json_params(params)
    branch = Branch.new    
    branch.id = params['BranchId']
    branch.name = params['Name']
    branch.description = params['Description']
    branch.product_id = params['ProductId']
    branch.path = params['Path']
    branch
  end
  def Branch.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']    
    hash[:Description] = params['description']
    hash[:ProductId] = params['product_id']  
    hash[:Path] = params['path']
    JSON.generate(hash)
  end
end
