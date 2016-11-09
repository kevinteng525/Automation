class Project < ActiveRecord::Base
  attr_accessor :vcs_server 
  attr_accessor :vcs_server_type
  attr_accessor :vcs_server_password
  attr_accessor :vcs_server_username
  attr_accessor :vcs_server_root_path
  attr_accessor :run_time
  VCSSERVER_TYPES = [['TFS',1],['ClearCase',2],['Shared Folder',3],['Git',4],['Not Sync',5]]
  RUN_TIME_TYPES = [['CSharpNUnit',0],['RubyMiniTest',1]]
  private
  #handle the communication with the Force Server
  def Project.get_force_project_by_id(id)
    response = force_server['projects/' + id.to_s].get
    Project.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Project.get_all_force_projects()
    list = Array.new    
    response = force_server['projects'].get
    params = JSON.parse(response.body)
    params.each do |param|
      list.push Project.initialize_from_json_params(param)      
    end
    list
  end
  
  def Project.create_force_project(params)
   
    response = force_server['projects/'].post Project.serialize_params_to_json(params),:content_type => 'application/json', :accept => 'application/json'
    Project.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Project.test_result_of_latest_task(id)
    response = force_server['projects/' + id.to_s + '/latestresultsummary'].get
    AutomationTaskRunningStatus.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Project.test_result_passrate_trend(id)
    response = force_server['projects/' + id.to_s + '/passratehistory'].get
    AutomationTaskRunningStatus.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Project.update_force_project(id, params)
    response = force_server['projects/' + id.to_s].put Project.serialize_params_to_json(params),:content_type => 'application/json', :accept=>'application/json'
    Project.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def Project.delete_force_project(id)
    response = force_server['projects/' + id.to_s].delete
  end
  
   def Project.get_force_supported_environments_for_project(id)
    list = Array.new 
    response = force_server['projects/' + id.to_s + '/supportedenvironments'].get
    if(response != '')
      params = JSON.parse(response.body)
      params.each do |param|
        list.push SupportedEnvironment.initialize_from_json_params(param)      
      end
      list      
    else
      nil
    end
   end

  def Project.initialize_from_json_params(params)
    project = Project.new
    project.id = params['ProjectId']
    project.vcs_server = params['VCSServer']
    project.vcs_server_type = params['VCSType']
    project.vcs_server_password = params['VCSPassword']
    project.vcs_server_username = params['VCSUser']
    project.vcs_server_root_path = params['VCSRootPath']
    project.run_time = params['RunTime']    
    project.name = params['Name']
    project.description = params['Description']
    project
  end
  
  def Project.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']    
    hash[:VCSRootPath] = params['vcs_server_root_path']
    #the project's runtime is decayed, we use the runtime of the test case instead
    hash[:RunTime] = 0 #RUN_TIME_TYPES[params['run_time'].to_i][0]
    hash[:VCSServer] = params['vcs_server'] 
    hash[:VCSUser] =  params['vcs_server_username'] 
    hash[:VCSType] = params['vcs_server_type'] 
    hash[:VCSPassword] = params['vcs_server_password']    
    hash[:Description] = params['description']
    JSON.generate(hash)
  end
end
