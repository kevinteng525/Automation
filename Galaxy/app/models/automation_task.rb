class AutomationTask < ActiveRecord::Base
  validate :BuildId, :EnvironmentId, :Name, :Priority, :RecurrencePattern, :StartDate, :StartTime, :TestContent, :Type, :WeekDays, :WeekInterval, :presence => true
  validate :Name, :uniqueness => true
  validate :start_time_must_be_a_future_time_for_one_time_run
  validate :week_days_must_be_great_than_0_for_weekly_run
  attr_accessor :project_id
  attr_accessor :notify_stakeholders
  attr_accessor :write_test_result_back
  attr_accessor :setup_script
  attr_accessor :teardown_script
  attr_accessor :enable_code_coverage
  
  
  AUTOMATION_TASK_TYPES = ['Normal','ByTestPlan']
  AUTOMATION_TASK_STATUS = ['Scheduled','Dispatched','Running','Completed','Failed','Paused','Cancelling','Cancelled','Scheduling', 'End']
  AUTOMATION_TASK_PRIORITIES = ['Highest','High','Medium','Low']
  
  AUTOMATION_TASK_SCHEDULE_PATTERN = ['As Soon As Possible','Once', 'Weekly']
  AUTOMATION_TASK_SCHEDULE_WEEK_DAYS = [['Sunday',1],['Monday',2],['Tuesday',4],['Wednesday',8],['Thursday',16],['Friday',32],['Saturday',64]]
  
  private
  
  def AutomationTask.get_force_automation_task_by_id(id)
    response = force_server['tasks/' + id.to_s].get
    AutomationTask.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def AutomationTask.send_test_result_report_of_task(id)
    response = force_server['tasks/' + id.to_s + '/reports'].get
  end
  
  def AutomationTask.get_force_test_suite_contains_all_failed_cases(id)
    response = force_server['tasks/' + id.to_s + '/suitesfailed'].get
    if response != ''
      TestSuite.initialize_from_json_params(JSON.parse(response.body))
    else
      nil
    end
  end
  
  def AutomationTask.get_all_force_automation_tasks()
    list = Array.new    
    response = force_server['tasks/'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push AutomationTask.initialize_from_json_params(param)      
      end
    end
    list
  end
  def AutomationTask.get_force_execution_progress_of_task(id)
    response = force_server['tasks/' + id.to_s + '/progress'].get
    AutomationTaskRunningStatus.initialize_from_json_params(JSON.parse(response.body))
  end
  
  
  def AutomationTask.get_force_execution_percentage_of_task(id)
    response = force_server['tasks/' + id.to_s + '/progress'].get
    Integer(JSON.parse(response.body)['ExecutionPercentage'])
  end
  
  def AutomationTask.get_force_execution_logs_of_task(id)
    response = force_server['tasks/' + id.to_s + '/log'].get
    response.body
  end
  
  def AutomationTask.create_force_automation_task(params)
    response = force_server['tasks/'].post AutomationTask.serialize_params_to_json(params),:content_type => 'application/json'
    AutomationTask.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def AutomationTask.update_force_automation_task(id, params)
    
    response = force_server['tasks/' + id.to_s].put AutomationTask.serialize_params_to_json(params),:content_type => 'application/json'
    
    AutomationTask.initialize_from_json_params(JSON.parse(response.body))
  end
  
  def AutomationTask.delete_force_automation_task(id)
    response = force_server['tasks/' + id.to_s].delete
  end  
 
 
  def AutomationTask.get_all_force_test_executions_for_task(id)
    list = Array.new
    response = force_server['tasks/' + id.to_s + '/testexecutions'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push TestCaseExecution.initialize_from_json_params(param)
      end
    end
    list
  end 
  
  def AutomationTask.get_all_force_automation_jobs_for_task(id)
    list = Array.new
    response = force_server['tasks/' + id.to_s + '/jobs'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push AutomationJob.initialize_from_json_params(param)
      end
    end
    list
  end 
  
  def AutomationTask.initialize_from_json_params(params)
    automation_task = AutomationTask.new
    automation_task.id = params['TaskId']
    automation_task.name = params['Name']
    automation_task.status = params['Status']
    automation_task.task_type = params['Type']
    automation_task.priority = params['Priority']
    automation_task.create_date = ms_json_to_date(params['CreateDate'])
    automation_task.create_by = params['CreateBy']
    automation_task.modify_date = ms_json_to_date(params['ModifyDate'])
    automation_task.modify_by = params['ModifyBy']
    automation_task.build_id = params['BuildId']   
    automation_task.supported_environment_id = params['EnvironmentId']   
    automation_task.test_content = params['TestContent']   
    automation_task.description = params['Description']
    automation_task.recurrence_pattern = params['RecurrencePattern']
    automation_task.start_date = ms_json_to_date(params['StartDate'])
    automation_task.start_time = ms_json_to_date(params['StartTime'])
    automation_task.week_days = params['WeekDays']
    automation_task.week_interval = params['WeekInterval']
    automation_task.branch_id = params['BranchId']
    automation_task.release_id = params['ReleaseId']
    automation_task.product_id = params['ProductId']
    automation_task.project_id = params['ProjectId']
    automation_task.parent_task_id = params['ParentTaskId']
    automation_task.write_test_result_back = params['WriteTestResultBack']
    automation_task.notify_stakeholders = params['NotifyStakeholders']
    automation_task.enable_code_coverage = params['EnableCodeCoverage']
    automation_task.setup_script = params['SetupScript']
    automation_task.teardown_script = params['TeardownScript']
    automation_task
  end
  
  def AutomationTask.serialize_params_to_json(params)
    hash = Hash.new
    hash[:Name] = params['name']
    hash[:Status] = params['status']
    hash[:Type] = params['task_type']
    hash[:Priority] = params['priority']
    hash[:CreateDate] = params['create_date']
    hash[:CreateBy] = params['create_by']
    hash[:ModifyDate] = params['modify_date']
    hash[:ModifyBy] = params['modify_by']
    hash[:BuildId] = params['build_id']
    hash[:EnvironmentId] = params['supported_environment_id']
    hash[:TestContent] = params['test_content']
    hash[:Description] = params['description'] 
    hash[:RecurrencePattern] = params['recurrence_pattern']
    hash[:StartDate] = params['start_date']
    hash[:StartTime] = params['start_time']
    hash[:WeekDays] = params['week_days']
    hash[:WeekInterval] = params['week_interval']
    hash[:BranchId] = params['branch_id']
    hash[:ReleaseId] = params['release_id']
    hash[:ProductId] = params['product_id']
    hash[:ProjectId] = params['project_id']
    hash[:ParentTaskId] = params['parent_task_id']
    hash[:WriteTestResultBack] = params['write_test_result_back']
    hash[:NotifyStakeholders] = params['notify_stakeholders']
    hash[:SetupScript] = params['setup_script']
    hash[:TeardownScript] = params['teardown_script']
    hash[:EnableCodeCoverage] = params['enable_code_coverage']
    JSON.generate(hash)
  end
  

  def start_time_must_be_a_future_time_for_one_time_run
    byebug
    start_date_time = Time.at((/\/Date\([0-9]+\)/.match(start_time)).to_i)
    errors.add(:start_time, "start time must be a future time.") unless start_date_time > Time.now
    errors.add(:start_date, "start time must be a future time.") unless start_date_time > Time.now
  end
  
  def week_days_must_be_great_than_0_for_weekly_run
    byebug
    if recurrence_pattern == 2
      errors.add(:week_days, "Please select at least one week day.") unless week_days > 0
    end
  end
  
end
