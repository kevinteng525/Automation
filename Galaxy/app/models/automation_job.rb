class AutomationJob < ActiveRecord::Base
  
  AUTOMATION_JOB_TYPES = ['NoDefined','Sequence','Concurrency']
  AUTOMATION_JOB_STATUS = ['Assigned','Preparing','Ready','Running','Completed','Failed','Paused','Cancelled','LockedBySaberAgent','Timeout','End']
  AUTOMATION_JOB_PRIORITIES = ['Highest','High','Medium','Low']
  
  private
  
  def AutomationJob.get_force_automation_job_by_id(id)
    response = force_server['jobs/' + id.to_s].get
    AutomationJob.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def AutomationJob.get_all_force_automation_jobs()
    list = Array.new    
    response = force_server['jobs/'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push AutomationJob.initialize_from_json_params(param)      
      end
    end
    list
  end
  def AutomationJob.get_force_job_progress_of_job(id)
    response = force_server['jobs/' + id.to_s + '/progress'].get
    response.body
  end
  
  
  def AutomationJob.get_force_test_cases_of_job(id)
    response = force_server['jobs/' + id.to_s + '/testcases'].get
    list = Array.new
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestCase.initialize_from_json_params(param)      
      end
    end
    list
  end
  
  def AutomationJob.get_force_execution_logs_of_job(id)
    response = force_server['jobs/' + id.to_s + '/log'].get
    response.body
  end
  
  def AutomationJob.create_force_automation_job(params)
    response = force_server['jobs/'].post AutomationJob.serialize_params_to_json(params),:content_type => 'application/json'
    AutomationJob.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def AutomationJob.update_force_automation_job(id, params)
    response = force_server['jobs/' + id.to_s].put AutomationJob.serialize_params_to_json(params),:content_type => 'application/json'
    AutomationJob.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def AutomationJob.delete_force_automation_job(id)
    response = force_server['jobs/' + id.to_s].delete
  end  
 
 
  def AutomationJob.get_all_force_test_executions_for_job(id)
    list = Array.new
    response = force_server['jobs/' + id.to_s + '/testexecutions'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestCaseExecution.initialize_from_json_params(param)
      end
    end
    list
  end 
  
  def AutomationJob.initialize_from_json_params(params)
    automation_job = AutomationJob.new
    automation_job.id = params['JobId']
    automation_job.name = params['Name']
    automation_job.status = params['Status']
    automation_job.job_type = params['Type']
    automation_job.priority = params['Priority']
    automation_job.created_at = ms_json_to_date(params['CreateDate'])
    automation_job.create_by = params['CreateBy']
    automation_job.updated_at = ms_json_to_date(params['ModifyDate'])
    automation_job.modify_by = params['ModifyBy']
    automation_job.retry_times = params['RetryTimes']
    automation_job.time_out = params['Timeout']
    automation_job.sut_environment_id = params['SUTEnvironmentId']
    automation_job.test_agent_environment_id = params['TestAgentEnvironmentId']  
    automation_job.description = params['Description'] 
    automation_job
  end
  
  def AutomationJob.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']
    hash[:Status] = params['status']
    hash[:Type] = params['job_type']
    hash[:Priority] = params['priority']
    #hash[:CreateDate] = params['create_date']
    hash[:CreateBy] = params['create_by']
    #hash[:ModifyDate] = params['modify_date']
    hash[:ModifyBy] = params['modify_by']
    hash[:RetryTimes] = params['retry_times']
    hash[:Timeout] = params['time_out']
    hash[:SUTEnvironmentId] = params['sut_environment_id']
    hash[:TestAgentEnvironmentId] = params['test_agent_environment_id']
    hash[:Description] = params['description']   
    JSON.generate(hash)
  end
  
end
