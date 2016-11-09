class User < ActiveRecord::Base
  validates :name, :presence=>true, :uniqueness=>true
  
  attr_accessor :email
  USER_TYPES = ['ATF','AD']
  USER_ROLES = ['SystemAdmin','User','Viewer']
  

  
  private
  
  #The logic to handle the communication with the Force service. 
  
  
  def User.get_force_user_by_id(user_id)    
    response = force_server['users/' + user_id.to_s].get
    if(response=='')
      nil
    else
      User.initiate_from_json_params(JSON.parse(response.body))
    end
  end 
  
  def User.get_all_force_users
    user_list = Array.new    
    response = force_server['users'].get
    params = JSON.parse(response.body)
    params.each do |param|
      user_list.push User.initiate_from_json_params(param)      
    end
    user_list
  end
  
  def User.create_force_user(params)      
    response = force_server['users/'].post User.serialize_params_to_json(params),:content_type => 'application/json'
    User.initiate_from_json_params(JSON.parse(response.body))
  end 
  
  
  def User.update_force_user(user_id,params) 
    response = force_server['users/' + user_id.to_s].put User.serialize_params_to_json(params),:content_type => 'application/json'
    User.initiate_from_json_params(JSON.parse(response.body))
  end
  
  def User.delete_force_user(user_id) 
    response = force_server['users/' + user_id.to_s].delete
  end  

  def User.get_subscribed_projects(user_id)
    subscribed_projects = Array.new
    response = force_server['users/' + user_id.to_s + '/subscribed_projects'].get
    params = JSON.parse(response.body)
    params.each do |param|
      subscribed_projects.push Project.initialize_from_json_params(param)
    end
    subscribed_projects
  end

  #help method
  
  def User.initiate_from_json_params(params)
    user = User.new
    user.id = params['UserId']
    user.email = params['Email']
    user.name = params['Username']
    user.password = params['Password']
    user.is_active = params['IsActive']
    user.description = params['Description']
    user.user_type = params['Type']
    user.role = params['Role']
    user
  end
  
  def User.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Username] = params['name']
    hash[:Email] = params['email']
    hash[:Password] = params['password']
    hash[:IsActive] = params['is_active']
    hash[:Type] = params['user_type']
    hash[:Description] = params['description']
    hash[:Role] = params['role']
    JSON.generate(hash)
  end
  
  def User.authenticate(name, password)
    hash = {:Username => name, :Password=>password}   
    begin
      response = force_server['users/isUserValid'].post JSON.generate(hash),:content_type => 'application/json'
      if(response.body == '')
        return nil
      else
        return User.initiate_from_json_params(JSON.parse(response.body))
      end
    rescue => e
      #the login is failed for internal errors, here we may need to log the failure
      return nil
    end
  end
end
