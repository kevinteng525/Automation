class TestCasesController < ApplicationController
  before_action :set_test_case, only: [:show, :edit, :update, :destroy]

  # GET /test_cases
  # GET /test_cases.json
  def index
    @test_cases = TestCase.get_all_force_test_cases
  end

  # GET /test_cases/1
  # GET /test_cases/1.json
  def show
  end

  # GET /test_cases/new
  def new
    @test_case = TestCase.new
  end

  # GET /test_cases/1/edit
  def edit
    def @test_case.new_record?()
      false
    end
  end

  # POST /test_cases
  # POST /test_cases.json
  def create
    # add the creator info
    test_case = TestCase.create_force_test_case(test_case_params)
    respond_to do |format|
      if test_case
        format.html { redirect_to test_case_path(test_case), notice: 'Test case was successfully created.' }
        format.json { render action: 'show', status: :created, location: @test_case }
      else
        format.html { render action: 'new' }
        format.json { render json: @test_case.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /test_cases/1
  # PATCH/PUT /test_cases/1.json
  def update
    test_case = TestCase.update_force_test_case(params[:id],test_case_params)
    respond_to do |format|      
      if test_case
        format.html { redirect_to test_case_path(test_case), notice: 'Test case was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @test_case.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /test_cases/1
  # DELETE /test_cases/1.json
  def destroy
    TestCase.delete_force_test_case(params[:id])
    respond_to do |format|
      format.html { redirect_to test_cases_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_test_case
      @test_case = TestCase.get_force_test_case_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def test_case_params
      params.require(:test_case).permit(:source_id, :name, :product_id, :feature, :script_path, :weight, :is_automated, :created_by, :modified_by, :description)
    end
end
