class TestResultsController < ApplicationController
  before_action :set_test_result, only: [:show, :edit, :update, :destroy]

  # GET /test_results
  # GET /test_results.json
  def index
    @test_results = TestResult.get_all_force_test_results
  end

  # GET /test_results/1
  # GET /test_results/1.json
  def show
  end

  # GET /test_results/new
  def new
    @test_result = TestResult.new
  end

  # GET /test_results/1/edit
  def edit
    def @test_result.new_record?()
      false
    end
  end

  # POST /test_results
  # POST /test_results.json
  def create
    @test_result = TestResult.create_force_test_result(test_result_params)

    respond_to do |format|
      if @test_result
        format.html { redirect_to test_result_path(@test_result), notice: 'Test result was successfully created.' }
        format.json { render action: 'show', status: :created, location: @test_result }
      else
        format.html { render action: 'new' }
        format.json { render json: @test_result.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /test_results/1
  # PATCH/PUT /test_results/1.json
  def update
    @test_result = TestResult.update_force_test_result(params[:id],test_result_params)
    respond_to do |format|
      if @test_result
        format.html { redirect_to test_result_path(@test_result), notice: 'Test result was successfully updated.' }
        format.json { head :no_content }
        format.js
      else
        format.html { render action: 'edit' }
        format.json { render json: @test_result.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /test_results/1
  # DELETE /test_results/1.json
  def destroy
    TestResult.delete_force_test_result(params[:id])
    respond_to do |format|
      format.html { redirect_to test_results_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_test_result
      @test_result = TestResult.get_force_test_result_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def test_result_params
      params.require(:test_result).permit(:execution_id, :result, :is_triaged, :triaged_by, :description, :files)
    end
end
