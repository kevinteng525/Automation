class AutomationJobsController < ApplicationController
  before_action :set_automation_job, only: [:show, :edit, :update, :destroy]

  # GET /automation_jobs
  # GET /automation_jobs.json
  def index
    @automation_jobs = AutomationJob.get_all_force_automation_jobs
  end

  # GET /automation_jobs/1
  # GET /automation_jobs/1.json
  def show
  end

  # GET /automation_jobs/new
  def new
    @automation_job = AutomationJob.new
  end

  # GET /automation_jobs/1/edit
  def edit
    def @automation_job.new_record?()
      false
    end
  end

  # POST /automation_jobs
  # POST /automation_jobs.json
  def create
    @automation_job = AutomationJob.new(automation_job_params)

    respond_to do |format|
      if @automation_job.save
        format.html { redirect_to @automation_job, notice: 'Automation job was successfully created.' }
        format.json { render action: 'show', status: :created, location: @automation_job }
      else
        format.html { render action: 'new' }
        format.json { render json: @automation_job.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /automation_jobs/1
  # PATCH/PUT /automation_jobs/1.json
  def update
    respond_to do |format|
      if @automation_job.update(automation_job_params)
        format.html { redirect_to @automation_job, notice: 'Automation job was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @automation_job.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /automation_jobs/1
  # DELETE /automation_jobs/1.json
  def destroy
    @automation_job.destroy
    respond_to do |format|
      format.html { redirect_to automation_jobs_url }
      format.json { head :no_content }
    end
  end

  # GET /automation_jobs/1/test_case_executions
  # GET /automation_jobs/1/test_case_executions.json
  def test_case_executions
    @test_case_executions = AutomationJob.get_all_force_test_executions_for_job(params[:id])
  end
  
  private
    # Use callbacks to share common setup or constraints between actions.
    def set_automation_job
      @automation_job = AutomationJob.find(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def automation_job_params
      params.require(:automation_job).permit(:name, :sut_environment_id, :test_agent_environment_id, :job_type, :priority, :status, :retry_times, :time_out, :create_by, :modify_by, :description)
    end
end
