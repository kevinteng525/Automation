class TestCaseExecutionsController < ApplicationController
  before_action :set_test_case_execution, only: [:show, :edit, :update, :destroy]

  # GET /test_case_executions
  # GET /test_case_executions.json
  def index
    @test_case_executions = TestCaseExecution.get_all_force_test_executions
  end

  # GET /test_case_executions/1
  # GET /test_case_executions/1.json
  def show
  end

  # GET /test_case_executions/new
  def new
    @test_case_execution = TestCaseExecution.new
  end

  # GET /test_case_executions/1/edit
  def edit
    def @test_case_execution.new_record?()
      false
    end
  end

  # POST /test_case_executions
  # POST /test_case_executions.json
  def create
    @test_case_execution = TestCaseExecution.create_force_test_execution(test_case_execution_params)

    respond_to do |format|
      if @test_case_execution
        format.html { redirect_to test_case_execution_path(@test_case_execution), notice: 'Test case execution was successfully created.' }
        format.json { render action: 'show', status: :created, location: @test_case_execution }
      else
        format.html { render action: 'new' }
        format.json { render json: @test_case_execution.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /test_case_executions/1
  # PATCH/PUT /test_case_executions/1.json
  def update
    @test_case_execution = TestCaseExecution.update_force_test_execution(params[:id],test_case_execution_params)
    respond_to do |format|
      if @test_case_execution
        format.html { redirect_to test_case_execution_path(@test_case_execution), notice: 'Test case execution was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @test_case_execution.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /test_case_executions/1
  # DELETE /test_case_executions/1.json
  def destroy
    TestCaseExecution.delete_force_test_execution
    respond_to do |format|
      format.html { redirect_to test_case_executions_url }
      format.json { head :no_content }
    end
  end

  def test_result
     @test_result = TestResult.get_force_test_result_by_execution_id(params[:id])
     @historical_results = TestResult.get_force_historical_test_results_by_testcase_id(TestCaseExecution.get_force_test_execution_by_id(params[:id]).test_case_id)
     respond_to do |format|
       format.html { render :partial=>'test_results/show_one_result'}
     end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_test_case_execution
      @test_case_execution = TestCaseExecution.get_force_test_execution_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def test_case_execution_params
      params.require(:test_case_execution).permit(:test_case_id, :job_id, :status, :start_time, :end_time, :retry_times)
    end
end
