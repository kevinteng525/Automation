
require 'date'
class AutomationTasksController < ApplicationController
  before_action :set_automation_task, only: [:show, :edit, :update, :destroy, :test_executions]

  # GET /automation_tasks
  # GET /automation_tasks.json
  def index
    @automation_tasks = AutomationTask.get_all_force_automation_tasks
  end

  # get /automation_tasks/1/test_executions
  def test_executions    
    
    @test_case_executions = AutomationTask.get_all_force_test_executions_for_task(params[:id])   
    respond_to do |format|
      if @test_case_executions
        format.html { render :partial=>'test_case_executions/test_case_executions_list_internal_for_task' }
        format.json {}#Todo
        format.js
      else
        format.html { render action: 'new' }
        format.json { render json: @automation_task.errors, status: :unprocessable_entity }
      end
    end
  end
  
  # get /automation_tasks/1/automation_jobs.json
  def automation_jobs
    @automation_jobs = AutomationTask.get_all_force_automation_jobs_for_task(params[:id])
    respond_to do |format|
      if @automation_jobs
        format.html { render :partial=>'automation_jobs/automation_jobs_compact' }
        format.json {}#Todo
        format.js
      else
        format.html { render action: 'new' }
        format.json { render json: @automation_task.errors, status: :unprocessable_entity }
      end
    end
  end

  # get /automation_tasks/1/rerun
  def rerun
    @old_temp_automation_task = AutomationTask.get_force_automation_task_by_id(params[:id])
    @old_automation_task = JSON.parse(@old_temp_automation_task.to_json)    
    @failed_testsuite = AutomationTask.get_force_test_suite_contains_all_failed_cases(params[:id])
    if @old_automation_task['recurrence_pattern'] != AutomationTask::AUTOMATION_TASK_SCHEDULE_PATTERN.index('As Soon As Possible')
      @automation_task = nil
      @notice = 'Please select the At Once task to rerun.'
    elsif @failed_testsuite
      if TestSuite.get_force_test_suite_by_id(@old_temp_automation_task.test_content).suite_type!=5
        @old_automation_task['test_content'] = @failed_testsuite.id
      end
      @old_automation_task['start_date'] = "\/Date(#{Time.now.to_i}000)\/"
      @old_automation_task['start_time'] = "\/Date(#{Time.now.to_i}000)\/"
      @old_automation_task['create_date'] = "\/Date(#{Time.now.to_i}000)\/"
      @old_automation_task['modify_date'] = "\/Date(#{Time.now.to_i}000)\/"
      @old_automation_task['name'] = '[Rerun] ' + @old_automation_task['name'] 
      @old_automation_task['status'] = AutomationTask::AUTOMATION_TASK_STATUS.index('Scheduled') 
      #it seems Rails will not handle the members we defined in the class by attr_access
      #the member is missed after to_json
      @old_automation_task['project_id']=@old_temp_automation_task.project_id
       @old_automation_task['enable_code_coverage']=@old_temp_automation_task.enable_code_coverage
      @old_automation_task['notify_stakeholders']=@old_temp_automation_task.notify_stakeholders
      @old_automation_task['write_test_result_back']=@old_temp_automation_task.write_test_result_back
      @old_automation_task['setup_script']=@old_temp_automation_task.setup_script
      @old_automation_task['teardown_script']=@old_temp_automation_task.teardown_script
      
      @automation_task = AutomationTask.create_force_automation_task(@old_automation_task)
      @notice = 'Successfully to submit the failed test cases to rerun.'
    else
      @automation_task = nil
      @notice = 'There are no failed test cases in this task. No need to rerun.'
    end
    respond_to do |format|
      if @automation_task        
        format.html {redirect_to automation_task_path(@automation_task), notice: @notice}
      else
        format.html {redirect_to automation_task_path(@old_automation_task), notice: @notice}
      end
    end
  end
  # get /automation_tasks/1/execution_progress
  def execution_progress
    @task_execution_progress = AutomationTask.get_force_execution_progress_of_task(params[:id])
  end

  # GET /automation_tasks/1
  # GET /automation_tasks/1.json
  def show
  end

  # GET /automation_tasks/new
  def new
    @automation_task = AutomationTask.new
    @rankings = Array.new
    TestCase.get_test_case_rankings().each do |ranking|
      @rankings.push(ranking)
    end
    @releases = Array.new
    TestCase.get_test_case_releases().each do |release|
      @releases.push(release)
    end
  end

  # GET /automation_tasks/1/edit
  def edit
    def @automation_task.new_record?()
      false
    end
  end

  # POST /automation_tasks
  # POST /automation_tasks.json
  def create
    temp_params = automation_task_params
    temp_params['create_by'] = session[:user_id].to_s
    temp_params['modify_by'] = session[:user_id].to_s
    temp_params['create_date'] = "\/Date(#{Time.now.to_i}000)\/"
    temp_params['modify_date'] = "\/Date(#{Time.now.to_i}000)\/"     
    @automation_task = AutomationTask.create_force_automation_task(temp_params)   
    respond_to do |format|
      if @automation_task
        format.html { redirect_to automation_task_path(@automation_task), notice: 'Automation task was successfully created.' }
        format.json { render action: 'show', status: :created, location: @automation_task }
      else
        format.html { render action: 'new' }
        format.json { render json: @automation_task.errors, status: :unprocessable_entity }
      end
    end
  end
  
  def cancel
    task = AutomationTask.get_force_automation_task_by_id(params[:id])
    cancelled_task = JSON.parse(task.to_json)
    cancelled_task["status"]=AutomationTask::AUTOMATION_TASK_STATUS.index('Cancelling')
    cancelled_task["create_date"] = "\/Date(#{task['create_date'].to_i}000)\/"
    cancelled_task['modify_date'] = "\/Date(#{task['modify_date'].to_i*1000})\/"
    cancelled_task['start_time'] = "\/Date(#{task['start_time'].to_i}000)\/"
    cancelled_task['start_date'] = "\/Date(#{task['start_date'].to_i}000)\/"  
    cancelled_task['project_id'] = task.project_id
    cancelled_task['notify_stakeholders'] = task.notify_stakeholders
     cancelled_task['enable_code_coverage'] = task.enable_code_coverage
    cancelled_task['write_test_result_back'] = task.write_test_result_back
    cancelled_task['setup_script'] = task.setup_script
    cancelled_task['teardown_script'] = task.teardown_script
    @automation_task = AutomationTask.update_force_automation_task(params[:id],cancelled_task)
    respond_to do |format|
      if @automation_task
        format.html { redirect_to automation_tasks_url }
        format.json { head :no_content }     
      end
     end    
  end
  
  # PATCH/PUT /automation_tasks/1.json
  # PATCH/PUT /automation_tasks/1
  def update   
    temp_params = automation_task_params
    temp_params['modify_by'] = session[:user_id].to_s
    @automation_task = AutomationTask.update_force_automation_task(params[:id],temp_params)
    respond_to do |format|
      if @automation_task
        format.html { redirect_to automation_task_path(@automation_task), notice: 'Automation task was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @automation_task.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /automation_tasks/1
  # DELETE /automation_tasks/1.json
  def destroy
    AutomationTask.delete_force_automation_task(params[:id])
    respond_to do |format|
      format.html { redirect_to automation_tasks_url }
      format.json { head :no_content }
    end
  end
  
  def report
    response = AutomationTask.send_test_result_report_of_task(params[:id])
    respond_to do |format|      
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_automation_task
      @automation_task = AutomationTask.get_force_automation_task_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def automation_task_params
      params.require(:automation_task).permit(:name, :status, :task_type, :priority, :create_date, :create_by, :modify_date, :modify_by, :build_id, :supported_environment_id, :test_content, :information, :description, :recurrence_pattern, :week_days, :start_date, :start_time, :week_interval, :parent_task_id, :branch_id, :release_id, :product_id, :project_id, :notify_stakeholders, :write_test_result_back, :setup_script, :teardown_script, :enable_code_coverage)
    end
end
